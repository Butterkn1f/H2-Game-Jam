using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using Common.DesignPatterns;

public class CustomerManager : Singleton<CustomerManager>
{
    // Character list to check for new customer
    private List<CustomerData> _customerList;
    private CustomerData _currentCustomer;
    public ReactiveProperty<CustomerData> CurrentCustomer = new ReactiveProperty<CustomerData>();

    // Make this into an object pool pls
    private GameObject _currentCustomerObject;

    // Level editable panels

    // Prefab for customer objects
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private GameObject _windowObject;


    public void Start()
    {
        _currentCustomer = null;
        _currentCustomerObject = null;
    }


    public void SetCustomerList(List<CustomerData> newCustomerList)
    {
        _customerList = newCustomerList;
    }

    /// <summary>
    /// This function gets a new customer from the list, as well as generates all the information about it
    /// </summary>
    public void SendNewCustomer()
    {
        // Don't send a new customer if the existing customer is still in the window
        if (_currentCustomerObject != null)
        {
            //return;

            // For testing only?? Probably should delete this soon
            _currentCustomerObject.GetComponent<CustomerAnimation>().PlayOutroAnimation();

        }

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

        _currentCustomerObject = Instantiate(_customerPrefab, _windowObject.transform);

        _currentCustomer = newCustomerData;
        _currentCustomerObject.GetComponent<Image>().sprite = _currentCustomer.CharacterSprite;

        // Generate order beforehand

        // Play intro animation (sliding in)
        _currentCustomerObject.GetComponent<CustomerAnimation>().PlayIntroAnimation();
        _currentCustomerObject.GetComponent<CustomerBehavior>().SetTimer(_currentCustomer.PatienceDuration);
    }

    /// <summary>
    /// Make the customer leave the current screen
    /// </summary>
    public void LeaveCurrentCustomer(bool sendNextCharacter = true)
    {
        if (sendNextCharacter)
        {
            SendNewCustomer();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SendNewCustomer();
        }
    }
}
