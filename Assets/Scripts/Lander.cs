using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {

private const float GRAVITY_NORMAL = 0.7f;
public static Lander Instance {get; private set;}
// create events
public event EventHandler OnUpForce;
public event EventHandler OnRightForce;
public event EventHandler OnLeftForce;
public event EventHandler OnBeforeForce;
public event EventHandler OnCoinPickup;
public event EventHandler OnFuelPickup;

// Create event to freeze timer after change state(after start)
public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
public class OnStateChangedEventArgs : EventArgs  {
    public State state;
  }

// Create event to landed score, create new class to give to event type of variable (int) instead of event with empty args
public event EventHandler<OnLandedEventArgs> OnLanded; 
public class OnLandedEventArgs : EventArgs  {
  public LandingType landingType;
  public int score; 
  public float dotVector;
  public float landingSpeed;
  public float scoreMultiplier;
  }

  // create enum to not make misstakes with new names of actions(landing)
public enum LandingType  {
    Success,
    WrongLandingArea,
    TooSteepAngle,
    TooFastLanding,
  }

public enum State {
    WaitingToStart,
    Normal,
    GameOver,
  }



  private Rigidbody2D landerRigidbody2D;
  private float fuelAmount;
  private float fuelAmountMax = 10f;
  private State state;

  private void Awake()  {

    Instance = this;
    // How much fuel on the start
    fuelAmount = fuelAmountMax;
    // waiting to start with 0 of gravity
    state = State.WaitingToStart;
    // add rigidbody to object from start game
    landerRigidbody2D = GetComponent<Rigidbody2D>();
    landerRigidbody2D.gravityScale = 0f;
    }




  private void FixedUpdate()    {
    if (GameInput.Instance == null) return;
    OnBeforeForce?.Invoke(this, EventArgs.Empty);

  // Check State after awake
    switch (state)
    {      default:
      case State.WaitingToStart:
        // pressing any arrows(input) gravity and state are becoming normal on the start of code
        // if (Keyboard.current.upArrowKey.isPressed ||
        //   Keyboard.current.rightArrowKey.isPressed ||
        //   Keyboard.current.leftArrowKey.isPressed){

        // pressing any arrows(input) when we create the GameInput script  and swap in other pieces of code
        if(GameInput.Instance.IsUpActionPressed() ||
          GameInput.Instance.IsRightActionPressed() ||
          GameInput.Instance.IsLeftActionPressed() ||
          GameInput.Instance.GetMovmentInputVector2() != Vector2.zero){
          landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
          SetState(State.Normal);
        } break;

      case State.Normal:
        //  No fuel
        if (fuelAmount <= 0f){
          return;
        }
          //  if press any key fuel will consume
        if (GameInput.Instance.IsUpActionPressed() ||
          GameInput.Instance.IsRightActionPressed() ||
          GameInput.Instance.IsLeftActionPressed()||
          GameInput.Instance.GetMovmentInputVector2() != Vector2.zero){
          ConsumeFuel();
        }

          // to add force when up arrow is pressed
          float gamepadDeadzone = .2f;
        if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.GetMovmentInputVector2().y > gamepadDeadzone) {
          float force = 700f;
          landerRigidbody2D.AddForce(force * transform.up*Time.deltaTime);
          
          // call to event upforce
          OnUpForce?.Invoke(this, EventArgs.Empty);
        }

          // to add force when left arrow is pressed
        if (GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.GetMovmentInputVector2().x < -gamepadDeadzone) {
          float turnLeft = +80f;
          landerRigidbody2D.AddTorque(turnLeft*Time.deltaTime);
              
          // call to event leftforce
          OnLeftForce?.Invoke(this, EventArgs.Empty);
        }

          // to add force when right arrow is pressed
        if ( GameInput.Instance.IsRightActionPressed() || GameInput.Instance.GetMovmentInputVector2().x > gamepadDeadzone){
          float turnRight = -80f;
          landerRigidbody2D.AddTorque(turnRight*Time.deltaTime);
            
          // call to event rightforce
          OnRightForce?.Invoke(this, EventArgs.Empty);
        }  break;
      case State.GameOver: break;

    }
  }


  // When lander is collision with landingPad
  private void OnCollisionEnter2D(Collision2D collision2D)
  {
    if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)){
      Debug.Log("CRASHED");
      OnLanded?.Invoke(this, new OnLandedEventArgs    {
      landingType = LandingType.WrongLandingArea,
      dotVector = 0f,
      landingSpeed = 0f,
      scoreMultiplier = 0f,
      score = 0,
    });
    SetState(State.GameOver);
      return;
    } 

    // Lander magnitude of arriving
    float softLandingVelocityMagnitude = 3f;
    float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
    if( relativeVelocityMagnitude > softLandingVelocityMagnitude){
      Debug.Log("Landed to hard");
      OnLanded?.Invoke(this, new OnLandedEventArgs    {
      landingType = LandingType.TooFastLanding,
      dotVector = 0f,
      landingSpeed = relativeVelocityMagnitude,
      scoreMultiplier = 0f,
      score = 0,
    });
    SetState(State.GameOver);
      return;
    }

    // Vector of arriving
    float dotVector = Vector2.Dot(Vector2.up, transform.up);
    float minDotVector = .95f;
    if(dotVector < minDotVector){
      Debug.Log("steep angle");
      OnLanded?.Invoke(this, new OnLandedEventArgs    {
      landingType = LandingType.TooSteepAngle,
      dotVector = dotVector,
      landingSpeed = relativeVelocityMagnitude,
      scoreMultiplier = 0f,
      score = 0,
    });
    SetState(State.GameOver);
      return;
    }

    Debug.Log("Successful");

    // Score of angle arriving
    float maxScoreAmountLandingAngle = 100f;
    float scoreDotVectorMultiplayer = 10f;
    float landigAngleScore = maxScoreAmountLandingAngle- Mathf.Abs(dotVector-1f)*scoreDotVectorMultiplayer*maxScoreAmountLandingAngle;

    // Score of Speed arriving
    float maxScoreAmountLandingSpeed = 100f;
    float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude)* maxScoreAmountLandingSpeed;

    Debug.Log("landigAngleScore " + landigAngleScore);
    Debug.Log("landingspeedScore" + landingSpeedScore);

    // Amount score
    int score = Mathf.RoundToInt((landigAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

    Debug.Log("score: " + score);
    OnLanded?.Invoke(this, new OnLandedEventArgs    {
      landingType = LandingType.Success,
      dotVector = dotVector,
      landingSpeed = relativeVelocityMagnitude,
      scoreMultiplier = landingPad.GetScoreMultiplier(),
      score = score,
    });
    SetState(State.GameOver);
  }

  private void OnTriggerEnter2D(Collider2D collider2D)  {
    // add fuel from fuel collision
    if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup)){
      float addFuelAmount = 10f;
      fuelAmount += addFuelAmount;
      if (fuelAmount > fuelAmountMax)       {
        fuelAmount = fuelAmountMax;
      }
      OnFuelPickup?.Invoke(this, EventArgs.Empty);
      fuelPickup.DestroySelf();
    }

      // add score when it's collision the coins
    if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup)){
      OnCoinPickup?.Invoke(this, EventArgs.Empty);
      coinPickup.DestroySelf();
    }

  }
  
  // method to call the event of change of state
  private void SetState(State state){
    this.state = state;
    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
      state = state,
    });
  }

  //Method of running out of fuel
  private void ConsumeFuel(){
    float fuelConsumptionAmount = 1f;
    fuelAmount -= fuelConsumptionAmount*Time.deltaTime;
  }
  // Methods of getting speed and fuel
  public float GetSpeedX()  {
    return landerRigidbody2D.linearVelocityX;
  }
  public float GetSpeedY()  {
    return landerRigidbody2D.linearVelocityY;
  }
  public float GetFuel()  {
    return fuelAmount;
  }

  public float GetFuelAmountNormalized()  {
    return fuelAmount/ fuelAmountMax;
  }

}
