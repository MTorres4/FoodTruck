using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodTruck;

namespace FoodTruckTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        //Confirm Roles
        public void Register_NewEmployee_EERolesConfirmed()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        //Confirm PayPal payment with Credit Card
        public void Pay_CreditCard_CreditCardDecreased()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        //Confirm PayPal payment with PayPal
        public void Pay_PayPal_PayPalDecreased()
        {
            //Arrange

            //Act

            //Assert
        }

        [TestMethod]
        //Confirm PayPal payment rec'd in account
        public void Admin_PaymentRecd_PayPalIncreased()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}
