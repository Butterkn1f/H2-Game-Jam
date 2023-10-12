using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using Common.DesignPatterns;

/// <summary>
/// This class manages the overall functions of the customers (as a whole)
/// </summary>
public class CustomerManager : Singleton<CustomerManager>
{
    // A reference list of all the customers
    private List<CustomerData> _customerList;
    private CustomerData _currentCustomer; // Current customer

    // TODO: Make this into an object pool pls
    public GameObject CurrentCustomerObject; // The current customer object

    [Tooltip("Window parent object (so that it spawns where we want it to)")]
    [SerializeField] private GameObject _windowObject;

    


    public void Start()
    {
        _currentCustomer = null;
        CurrentCustomerObject = null;

        
    }

    /// <summary>
    /// Sets the list of customers that we can choose from
    /// For area specific customers 
    /// </summary>
    /// <param name="newCustomerList">List of customers to be randomised from</param>
    public void SetCustomerList(List<CustomerData> newCustomerList)
    {
        _customerList = newCustomerList;
    }

    /// <summary>
    /// This function gets a new customer from the list, as well as generates all the information about it
    /// </summary>
    public void SendNewCustomer()
    {
        // initialise temporary variables
        CustomerData newCustomerData = null;
        bool temporaryCheck = false;

        // Error check
        if (_customerList.Count == 0)
        {
            Debug.LogError("Customer List is empty/ does not have enough characters. Please add a customer data SO into the location data plox");
        }
        
        // Randomise to get a new customer
        while (!temporaryCheck)
        {
            newCustomerData = _customerList[Random.Range(0, _customerList.Count)];
            // If there is no current customer or if there are not enough characters
            // Pass the randomisation test
            if (_currentCustomer == null)
            {
                temporaryCheck = true;
            }
            else
            {
                // Use the name as a unique identifier for the customer (make sure u dont get the same customer 2 times in a row)
                // Additionally, check if its even possible to do non-alternating customers
                if (newCustomerData.Name != _currentCustomer.Name || _customerList.Count == 1)
                {
                    temporaryCheck = true;
                }
            }
        }

        _currentCustomer = newCustomerData;

        // Spawn a new customer object
        CurrentCustomerObject = Instantiate(_currentCustomer.CustomerPrefab, _windowObject.transform);

        // Give it the correct data

        MainGameManager.Instance.GameState.SetValue(MainGameState.GAME_PREPARE);

        // Play intro animation (sliding in)
        StartCoroutine(IntroAnimationSeqence());
    }

    /// <summary>
    /// The main character introduction animation
    /// </summary>
    /// <param name="IntroDelay">Any delays to be played (in case you want to delay for some reason)</param>
    private IEnumerator IntroAnimationSeqence(float IntroDelay = 0)
    {
        yield return new WaitForSeconds(IntroDelay);

        CurrentCustomerObject.GetComponent<CustomerAnimation>().PlayIntroAnimation(); // Play customer slide in

        yield return new WaitForSeconds(0.5f);

        CurrentCustomerObject.GetComponent<CustomerUI>().IntroAnim(); // Speech bubble pop

        yield return new WaitForSeconds(CurrentCustomerObject.GetComponent<CustomerUI>().IntroAnimationDuration);

        CurrentCustomerObject.GetComponent<CustomerBehavior>().SetTimer(_currentCustomer.PatienceDuration); // Start the timer after animation is done
    }

    private IEnumerator OutroAnimationSequence(bool sendNextCharacter = true)
    {
        // For testing only?? Probably should delete this soon
        CurrentCustomerObject.GetComponent<CustomerUI>().OutroAnim();

        yield return new WaitForSeconds(0.25f);

        CurrentCustomerObject.GetComponent<CustomerAnimation>().PlayOutroAnimation();


        if (sendNextCharacter)
        {
            SendNewCustomer();
        }
    }

    /// <summary>
    /// Make the customer leave the current screen
    /// </summary>
    public void LeaveCurrentCustomer(bool sendNextCharacter = true)
    {
        MainGameManager.Instance.GameState.SetValue(MainGameState.GAME_WAIT);
        StartCoroutine(OutroAnimationSequence(sendNextCharacter));
    }

    private void Update()
    {
        
    }
}
