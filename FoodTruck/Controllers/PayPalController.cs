using FoodTruck.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PayPalController : Controller
    {
        // GET: PayPal
        public ActionResult Index()
        {
            return RedirectToAction("PayPal", "Admin");
        }

        public ActionResult PaymentWithCreditCard()
        {
            Item item = new Item();
            item.name = "Demo Item";
            item.currency = "USD";
            item.price = "5";
            item.quantity = "1";


            List<Item> items = new List<Item>();
            items.Add(item);
            ItemList itemList = new ItemList();
            itemList.items = items;

            CreditCard creditcard = new CreditCard();
            creditcard.number = "4032035983291196";
            creditcard.type = "VISA";
            creditcard.expire_month = 3;
            creditcard.expire_year = 2022;
            creditcard.payer_id = "iBuy1@example.com";

            Details details = new Details();
            details.subtotal = "5";
            details.tax = "1";

            Amount amount = new Amount();
            amount.currency = "USD";
            //Total = shipping tax + subtotal
            amount.total = "6";
            amount.details = details;

            Transaction trans = new Transaction();
            trans.amount = amount;
            trans.description = "Description about the payment amount";
            trans.item_list = itemList;

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(trans);

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = creditcard;

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            Payer pay = new Payer();
            pay.funding_instruments = fundingInstrumentList;
            pay.payment_method = "credit_card";

            Payment payment = new Payment();
            payment.intent = "sale";
            payment.payer = pay;
            payment.transactions = transactions;

            try
            {
                APIContext apiContext = Configuration.GetAPIContext();
                Payment createdPayment = payment.Create(apiContext);

                if (createdPayment.state.ToLower() != "approved")
                {
                    return RedirectToAction("Failure", "Admin");
                }
            }
            catch (PayPal.PayPalException ex)
            {
                return RedirectToAction("Failure", "Admin");
            }
            return RedirectToAction("Success", "Admin");
        }

        public ActionResult PaymentWithPayPal()
        {
            APIContext apiContext = Configuration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                        "/Paypal/PaymentWithPayPal?";

                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("Failure", "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Failure", "Admin");
            }
            return RedirectToAction("Success", "Admin");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var itemList = new ItemList() { items = new List<Item>() };
            itemList.items.Add(new Item()
            {
                name = "Item Name",
                currency = "USD",
                price = "6",
                quantity = "1"
            });

            var payer = new Payer() { payment_method = "paypal" };
            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "1",
                subtotal = "6"
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = "7",
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "Sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirectUrls
            };

            return this.payment.Create(apiContext);
        }
    }
}
