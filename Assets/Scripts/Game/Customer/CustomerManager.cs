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
[RequireComponent(typeof(CustomerAnimation))]
[RequireComponent(typeof(CustomerUI))]
[RequireComponent(typeof(CustomerBehavior))]
public class CustomerManager : Singleton<CustomerManager>
{
    // A reference list of all the customers
    private List<CustomerData> _customerList;
    private CustomerData _currentCustomer; // Current customer

    // TODO: Make this into an object pool pls
    private GameObject _currentCustomerObject; // The current customer object

    // Prefab for customer objects
    [Tooltip("Customer prefab to be instantiated")]
    [SerializeField] private GameObject _customerPrefab; // Get a reference for the customer prefab (to be instantiated)

    [Tooltip("Window parent object (so that it spawns where we want it to)")]
    [SerializeField] private GameObject _windowObject;


    public void Start()
    {
        _currentCustomer = null;
        _currentCustomerObject = null;
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

        // Spawn a new customer object
        _currentCustomerObject = Instantiate(_customerPrefab, _windowObject.transform);

        // Give it the correct data
        _currentCustomer = newCustomerData;
        _currentCustomerObject.GetComponent<Image>().sprite = _currentCustomer.CharacterSprite;

        // TO LI LIAN: Generate an order here maybe? 
        // TO LI LIAN: Use the customer UI to display the current dish

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

        _currentCustomerObject.GetComponent<CustomerAnimation>().PlayIntroAnimation(); // Play customer slide in

        yield return new WaitForSeconds(0.5f);

        _currentCustomerObject.GetComponent<CustomerUI>().IntroAnim(); // Speech bubble pop

        yield return new WaitForSeconds(_currentCustomerObject.GetComponent<CustomerUI>().IntroAnimationDuration);

        _currentCustomerObject.GetComponent<CustomerBehavior>().SetTimer(_currentCustomer.PatienceDuration); // Start the timer after animation is done
    }

    private IEnumerator OutroAnimationSequence(bool sendNextCharacter = true)
    {
        // For testing only?? Probably should delete this soon
        _currentCustomerObject.GetComponent<CustomerUI>().OutroAnim();
        yield return new WaitForSeconds(0.25f);
        _currentCustomerObject.GetComponent<CustomerAnimation>().PlayOutroAnimation();


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
        StartCoroutine(OutroAnimationSequence(sendNextCharacter));
    }

    private void Update()
    {
        
    }
}