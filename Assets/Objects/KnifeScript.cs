using UnityEngine;

public class KnifeScript : MonoBehaviour
{

    [SerializeField]
    private Vector2 throwForce;

    //knife shouldn't be controlled by the player when it's inactive 
    //(i.e. it already hit the Mushroom / another knife)
    private bool isActive = true;

    //for controlling physics
    private Rigidbody2D rb;
    //the collider attached to Knife
    private BoxCollider2D knifeCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knifeCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        //this method of detecting input also works for touch
        if (Input.GetMouseButtonDown(0) && isActive)
        {
            //"throwing" the knife
            rb.AddForce(throwForce, ForceMode2D.Impulse);
            //once the knife isn't stationary, we can apply gravity (it will not automatically fall down)
            rb.gravityScale = 1;
            //Decrement number of available knives
            GameController.Instance.GameUI.DecrementDisplayedKnifeCount();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //we don't even want to detect collisions when the knife isn't active
        if (!isActive)
            return;

        //if the knife happens to be active (1st collision), deactivate it
        isActive = false;

        //collision with a log
        if (collision.collider.tag == "Mushroom")
        {
            //play the particle effect on collision,
            //you don't always have to store the component in a field...
            GetComponent<ParticleSystem>().Play();

            //stop the knife
            rb.velocity = new Vector2(0, 0);
            //this will automatically inherit rotation of the new parent (Mushroom)
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(collision.collider.transform);

            //move the collider away from the blade which is stuck in the Mushroom
            knifeCollider.offset = new Vector2(knifeCollider.offset.x, -0.4f);
            knifeCollider.size = new Vector2(knifeCollider.size.x, 1.2f);

            //Spawn another knife
            GameController.Instance.OnSuccessfulKnifeHit();
        }
        //collision with another knife
        else if (collision.collider.tag == "Knife")
        {
            //start rapidly moving downwards
            rb.velocity = new Vector2(rb.velocity.x, -2);
            //Game Over
            GameController.Instance.StartGameOverSequence(false);
        }
    }
}