using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CustomerManager : MonoBehaviour
{
    // Character list to check for new customer
    private List<CustomerData> _customerList;
    private CustomerData _currentCustomer;
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
    /// This function gets a new customer from the list
    /// </summary>
    public void SendNewCustomer()
    {
        // Play exit animation for the previous customer
        if (_currentCustomerObject != null)
        {
            _currentCustomerObject.GetComponent<CustomerAnimation>().PlayOutroAnimation();
        }

        CustomerData newCustomerData = null;
        bool temporaryCheck = false;

        // Error check
        if (_customerList.Count == 0)
        {
            Debug.LogError("Customer List is empty/ does not have enough characters. Please add a customer data SO into the location data plox");
        }
        
        // Randomise one 
        while (!temporaryCheck)
        {
            newCustomerData = _customerList[Random.Range(0, _customerList.Count)];
            // If there is no current customer or if there are not enough characters
            // Pass the randomisation test
            if (_currentCustomer != null)
            {
                temporaryCheck = true;
            }
            else
            {
                // Use the name as a unique identifier for the customer (make sure u dont get the same customer 2 times in a row)
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

        // Play animation here
        _currentCustomerObject.GetComponent<CustomerAnimation>().PlayIntroAnimation();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SendNewCustomer();
        }
    }
}
