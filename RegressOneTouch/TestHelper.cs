using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RegressOneTouch
{
    class TestHelper // Обертка для Selenium
    {
        private const int sleeper = 1800;

        public bool DefaultStation(IWebDriver Chrome, string defaultURL)  // Открывает страницу, разворачивает окно.
        {
            bool result;
            try
            {
                Chrome.Manage().Window.Maximize();
                Chrome.Navigate().GoToUrl(defaultURL);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool SendDataToAddressString(IWebDriver Chrome, string Datastring) // Заполлнение поля ввода адреса  и выбор адреса
        {
            bool result = true;
            try
            {
                Chrome.FindElement(By.CssSelector(".ui-autocomplete-input")).Clear();
                Chrome.FindElement(By.CssSelector(".ui-autocomplete-input")).SendKeys(Datastring);
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElement(By.CssSelector(".ui-autocomplete-input")).SendKeys(Keys.Enter);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void DriverClose(IWebDriver Chrome) // Выход из Chrome
        {
            try
            {
                Chrome.Quit();
            }
            catch
            {
                Console.WriteLine("Не удалось закрыть браузер");
            }
        }

        public bool Autorise(IWebDriver Chrome, string TestPhone, string TestPassword) // Авторизация под логином и паролем
        {
            bool result = true;
            Chrome.FindElement(By.CssSelector(".header-auth__link")).Click();
            System.Threading.Thread.Sleep(sleeper);
            Chrome.FindElements(By.CssSelector(".mb5 input.js-mask-phone"))[0].Clear();
            Chrome.FindElements(By.CssSelector(".mb5 input.js-mask-phone"))[0].SendKeys(TestPhone);
            Chrome.FindElements(By.CssSelector(".mb15 .form-control"))[0].SendKeys(TestPassword);
            Chrome.FindElements(By.CssSelector("button.btn.btn-block.btn-lg.btn-dark.text-uppercase.b.js-form-submit"))[0].Click();
            System.Threading.Thread.Sleep(sleeper);
            if (Chrome.FindElement(By.CssSelector(".header-auth .b")).Text == "Вход")
            {
                result = false;
            }            
            return result;
        }

        public void CloseLocalisePopap(IWebDriver Chrome) // Закрывает попап локализации 
        {
            Chrome.FindElements(By.CssSelector(".btn-sm"))[0].Click();
        }

        public bool AddToBasket(IWebDriver Chrome) // Добавляет случайный товар в корзину 
        {
            Chrome.Navigate().Refresh();
            bool result = false;
            string number = " "; // Кол-во уже добавленного товара
            if (Chrome.FindElement(By.CssSelector(".header-cart__count")).Displayed) // Если уже есть добавленные товары
                number = Chrome.FindElement(By.CssSelector(".header-cart__count")).Text;

            try // Добавление товара
            {
                Random rnd = new Random();
                List<IWebElement> Elements = Chrome.FindElements(By.CssSelector("div.catalog-item__btn")).ToList();
                // Копим товары в корзине               
                int value = rnd.Next(0, Elements.Count - 1);
                Elements[value].Click();                
            }
            catch
            {
                Console.WriteLine("Не корректное добавление в корзину товаров");
                return result;
            }
            System.Threading.Thread.Sleep(sleeper);
            try  // Проверка корректноести добавления
            {
                if (number != " ")
                {
                    if (Convert.ToInt32(Chrome.FindElement(By.CssSelector(".header-cart__count")).Text) == Convert.ToInt32(number) + 1)
                    {
                        Console.WriteLine("Корректное добавление в корзину товаров");
                        result = true;
                    }
                    else
                    {
                        Console.WriteLine("Не все товары добавлены");
                    }
                }
                else
                {
                    if (Chrome.FindElement(By.CssSelector(".header-cart__count")).Text == "1")
                    {
                        Console.WriteLine("Корректное добавление в корзину товаров");
                        result = true;
                    }
                    else
                    {                        
                        Console.WriteLine("Не все товары добавлены");                        
                    }
                }
            }
            catch
            {
                Console.WriteLine("Не корректное добавление в корзину товаров");
                return result;
            }
            return result;
        }

        public bool OpenBasket(IWebDriver Chrome, string baseURL) // Открытие корзины 
        {
            bool result = true;
            try
            {
                Chrome.FindElement(By.CssSelector(".header-cart__sum")).Click();
                List<IWebElement> Elements = Chrome.FindElements(By.CssSelector("tbody div.form-group")).ToList();
                System.Threading.Thread.Sleep(sleeper);
                Elements[Elements.Count - 2].Click();
                Console.WriteLine("Удачное открытие корзины");
                return result;
            }
            catch
            {
                Console.WriteLine("Не удачное открытие корзины");
                return result;
            }
        }

        public bool OpenRegistration(IWebDriver Chrome) // Открытие попапа авторизации/регистрации и выбор вкладки "регистрация"
        {
            bool result = false;
            try
            {
                Chrome.FindElement(By.CssSelector(".header-auth__link")).Click();
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElements(By.ClassName("auth-tabs__item"))[1].Click();
                if (Chrome.FindElement(By.CssSelector(".js-sms-confirm .text-uppercase")).Text.Contains("зарегистрироваться"))
                {
                    result = true;
                }
            }
            catch
            {

            }            
            return result;
        }

        public bool CheckItemsToBasket(IWebDriver Chrome) // Проверка соответствия товаров в корзине 
        {
            bool result = true;
            Chrome.FindElement(By.CssSelector(".header-cart__sum")).Click();
            System.Threading.Thread.Sleep(sleeper);
            List<IWebElement> Elements = Chrome.FindElements(By.CssSelector(".table__cart-item .col-md-6")).ToList();
            for (int i = 0; i<Elements.Count/2; ++i)
            {                
                if (Elements[i].Text != Elements[i+Elements.Count/2].Text)
                {
                    result = false;                    
                    break;
                }
            }
            return result;
        }

    }
}
