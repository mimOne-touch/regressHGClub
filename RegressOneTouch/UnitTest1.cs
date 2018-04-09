using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RegressOneTouch
{
    [TestClass]
    public class UnitTest1
    {
        static string baseURL;
        private static int wait_time; // кол-во секунд для ожидания элемента на странице
        private static int sleeper; // Доп переменная, регулирует скорость работы тестов
        private static ChromeDriver Chrome; // тест
        private static TestHelper TH;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext) // 
        {
            //baseURL = "http://hgclub.site.dev.one-touch.ru/";
            //baseURL = "http://hgclub.lsp.dev.one-touch.ru/catalog/";
            //baseURL = "http://hgclub.test.dev.one-touch.ru/catalog/";
            baseURL = "https://hgclub.ru/";
            wait_time = 5;
            TimeSpan interval = new TimeSpan(0, 0, wait_time);            
            sleeper = 1500;
            Chrome = new ChromeDriver();
            Chrome.Manage().Timeouts().ImplicitlyWait(interval);
            TH = new TestHelper();
        }

        [TestMethod]
        public void BasketDropAddress() //Дефект https://trello.com/c/qAUpb8lF  https://trello.com/c/uXB5dnxr
        {
            // Открываем страницу
            try
            {
                TH.DefaultStation(Chrome, baseURL + "catalog/");
                Console.WriteLine("Корректная инициализация драйвера");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера");
                Assert.IsTrue(false);
            }
            // Заполняем поле ввода адреса
            try
            {
                TH.SendDataToAddressString(Chrome, "Москва, Ленинский проспект, 99");
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElement(By.CssSelector(".main-confirm-btn")).Click();
            }
            catch
            {
                Console.WriteLine("Не корректное заполнение поля ввода адреса");
                Assert.IsTrue(false);
            }
            Chrome.Navigate().Refresh();
            try
            {
                if (!Chrome.FindElement(By.CssSelector(".error-hidden")).Displayed)
                {
                    Console.WriteLine("Не корректное заполнение поля ввода адреса");
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                Console.WriteLine("Корректное заполнение поля ввода адреса");
            }
            //Добавляем в корзину товары (4 товара)
            try
            {
                Random rnd = new Random();
                List<IWebElement> Elements = Chrome.FindElements(By.CssSelector("div.catalog-item__btn")).ToList();
                for (int i = 0; i < 2; i++)
                {
                    int value = rnd.Next(0, Elements.Count - 1);
                    Elements[value].Click();
                }
            }
            catch
            {
                Console.WriteLine("Не корректное добавление в корзину товаров");
                Assert.IsTrue(false);
            }
            try
            {
                Chrome.Navigate().Refresh();
                if (Chrome.FindElement(By.CssSelector(".header-cart__count")).Text == "2")
                {
                    Console.WriteLine("Корректное добавление в корзину товаров");
                }
                else
                {
                    Console.WriteLine("Не все товары добавлены");
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                Console.WriteLine("Не все товары добавлены");
                Assert.IsTrue(false); // При возникновении, провести тест повторно
            }
            // Попытка сменить адрес и вызвать ошибку
            try
            {
                TH.SendDataToAddressString(Chrome, "Москва, Ленинский проспект, 99, подъезд 1");
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElement(By.CssSelector(".main-confirm-btn")).Click();
                if (Chrome.FindElement(By.CssSelector(".header-cart__count")).Text == "2")
                {
                    Console.WriteLine("Баг не повторился, регресса нет");                    
                }
                else
                {
                    Console.WriteLine("Произошел регресс!\nДефекты https://trello.com/c/uXB5dnxr https://trello.com/c/qAUpb8lF ");
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void Add_0_Product()    // Дефект https://trello.com/c/FLjp3WaR 
        {
            try
            {   // Инициализация
                TH.DefaultStation(Chrome, baseURL + "catalog/");
                TH.SendDataToAddressString(Chrome, "Москва, Ленинский проспект, 99");
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElement(By.CssSelector(".main-confirm-btn")).Click();
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера");
                Assert.IsTrue(false);
            }
            Random rnd = new Random();
            int value = 0;
            try
            {   // Жмем по рандомному знаку "-"                
                List<IWebElement> Elements = Chrome.FindElements(By.CssSelector(".addon--minus")).ToList();
                value = rnd.Next(0, Elements.Count);
                Elements[value].Click();
            }
            catch
            {
                Console.WriteLine("Не корректное нажатие на кнопку '-' ");
                Assert.IsTrue(false);
            }
            try
            {
                List<IWebElement> Elements = Chrome.FindElements(By.CssSelector(".input-group__num")).ToList();
                if (Elements[value].Text == "0")
                {
                    Console.WriteLine("Баг повторился, регресс \nДефект: https://trello.com/c/FLjp3WaR ");
                    Assert.IsTrue(false);
                }
                else
                {
                    Console.WriteLine("Баг не повторился");
                }
            }
            catch
            {
                Console.WriteLine("Не корректная обработка страницы");
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void MassageAneedCity() // Дефект https://trello.com/c/U559HK6x 
        {
            try
            {
                TH.DefaultStation(Chrome, baseURL);
                Console.WriteLine("Корректная инициализация драйвера");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера");
                Assert.IsTrue(false);
            }
            try
            {
                TH.SendDataToAddressString(Chrome, "");
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElement(By.CssSelector(".main-confirm-btn")).Click();
            }
            catch
            {
                Console.WriteLine("Не нажимается конопка подтверждения адреса");
                Assert.IsTrue(false);
            }
            try
            {

                if (Chrome.FindElement(By.CssSelector(".error-hidden")).Text != "Укажите город")
                {
                    Console.WriteLine("Баг повторился, регресс \nДефект: https://trello.com/c/U559HK6x");
                    Assert.IsTrue(false);
                }
                else
                {
                    Console.WriteLine("Баг не повторился");
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [DataTestMethod]       
        [DataRow("")]
        [DataRow("test")]
        [DataRow("testtes ttest")]
        [DataRow(" '; ]} >,< ")]
        public void MasageValidationPassword(string testPassword) // Дефект https://trello.com/c/qrWe9co3
        {
            try
            {
                TH.DefaultStation(Chrome, baseURL);
                if (Chrome.FindElement(By.CssSelector(".header-auth .b")).Text == "Вход")
                {
                    TH.Autorise(Chrome, "79997001301", "qwerty");
                }
                Chrome.FindElement(By.CssSelector(".header-auth .b")).Click();
                Console.WriteLine("Корректная инициализация драйвера и авторизация");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера или авторизация");
                Assert.IsTrue(false);
            }
            try
            {   // заполняем пароли
                Chrome.FindElements(By.CssSelector(".col-md-10 .col-md-6 .form-control"))[0].SendKeys(testPassword);
                Chrome.FindElements(By.CssSelector(".col-md-10 .col-md-6 .form-control"))[1].SendKeys(testPassword);
            }
            catch
            {
                Console.WriteLine("Не корректное заполнение паролей");
                Assert.IsTrue(false);
            }
            try
            {   //Жмем сохранить 
                Chrome.FindElements(By.CssSelector(".col-md-6 .btn-dark"))[1].Click();
                if (Chrome.FindElements(By.CssSelector(".col-md-10 .col-md-6 .form-control"))[0].Text == "")
                {
                    Console.WriteLine("Баг повторился, регресс \nДефект: https://trello.com/c/qrWe9co3"); // пароль принят
                    // Восстанавливаем пароль на дефолтный "qwerty"
                    Chrome.FindElements(By.CssSelector(".col-md-10 .col-md-6 .form-control"))[0].SendKeys("qwerty");
                    Chrome.FindElements(By.CssSelector(".col-md-10 .col-md-6 .form-control"))[1].SendKeys("qwerty");
                    Chrome.FindElements(By.CssSelector(".col-md-6 .btn-dark"))[1].Click();
                    Assert.IsTrue(false);
                }
                else
                {
                    Console.WriteLine("Баг не повторился");
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("test")]
        [DataRow("testtes ttest")]
        [DataRow(" '; ]} >,< ")]
        [DataRow("@")]
        public void MasageValidationEmail(string testEmail) // Дефект https://trello.com/c/mfJP8BaV 
        {
            try
            {
                TH.DefaultStation(Chrome, baseURL);
                if (Chrome.FindElement(By.CssSelector(".header-auth .b")).Text == "Вход")
                {
                    TH.Autorise(Chrome, "79997001301", "qwerty");
                }
                Chrome.FindElement(By.CssSelector(".header-auth .b")).Click();
                Console.WriteLine("Корректная инициализация драйвера и авторизация");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера или авторизация");
                Assert.IsTrue(false);
            }
            try
            {
                Chrome.FindElements(By.CssSelector(".col-md-10 .form-control"))[1].Clear();
                Chrome.FindElements(By.CssSelector(".col-md-10 .form-control"))[1].SendKeys(testEmail);
            }
            catch
            {
                Console.WriteLine("Не корректное заполнение e-mail");
                Assert.IsTrue(false);
            }
            try
            {
                //Жмем сохранить 
                Chrome.FindElements(By.CssSelector(".col-md-6 .btn-dark"))[1].Click();
                if (Chrome.FindElement(By.ClassName("error_hidden")).Text != "Введите")
                {
                    Console.WriteLine("Баг повторился, регресс \nДефект: https://trello.com/c/mfJP8BaV");
                    Assert.IsTrue(false);
                }
                else
                {
                    Console.WriteLine("Баг не повторился");
                }
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void TipForPoints()  // Дефект https://trello.com/c/joiW3mcq
        {
            try // Авторизация для наличия баллов
            {
                TH.DefaultStation(Chrome, baseURL);
                if (Chrome.FindElement(By.CssSelector(".header-auth .b")).Text == "Вход")
                {
                    TH.Autorise(Chrome, "79997001301", "qwerty");
                }
                Console.WriteLine("Корректная инициализация драйвера и авторизация");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера или авторизация");
                Assert.IsTrue(false);
            }
            try // Ввод адреса
            {
                TH.SendDataToAddressString(Chrome, "Москва, Ленинский проспект, 99");
                System.Threading.Thread.Sleep(sleeper);
                Chrome.FindElement(By.CssSelector(".main-confirm-btn")).Click();
            }
            catch
            {
                Console.WriteLine("Не корректное заполнение поля ввода адреса");
                Assert.IsTrue(false);
            }
            try
            {   // добавляем три случайных товара 
                Chrome.Navigate().GoToUrl(baseURL + "catalog/");
                Assert.IsTrue(TH.AddToBasket(Chrome));
                Assert.IsTrue(TH.AddToBasket(Chrome));
                Assert.IsTrue(TH.AddToBasket(Chrome));
                Assert.IsTrue(TH.AddToBasket(Chrome));
            }
            catch
            {
                Console.WriteLine("Не корректное добавление товаров в корзину");
                Assert.IsTrue(false);
            }
            try
            {   
                Assert.IsTrue(TH.OpenBasket(Chrome,baseURL));
                // Проверяем поле e-mail на корректность 
                if (!Chrome.FindElements(By.CssSelector(".col-md-10 .form-control"))[1].Text.Contains("@."))
                {
                    Chrome.FindElements(By.CssSelector(".col-md-10 .form-control"))[1].Clear();
                    Chrome.FindElements(By.CssSelector(".col-md-10 .form-control"))[1].SendKeys("test@te.ru");
                }
            }
            catch
            {
                Console.WriteLine("Не корректное открытие корзины");
                Assert.IsTrue(false);
            }
            try
            {
                Chrome.FindElement(By.CssSelector(".btn-lg")).Click();
                if (Chrome.Url == baseURL + "basket/order/")
                {
                    Console.WriteLine("Удачный переход ко второму шагу");
                }
                else
                {
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                Console.WriteLine("Не удачный переход ко второму шагу");
                Assert.IsTrue(false);
            }
            try 
            { // Проверяем, привильно ли добавлены товары к заказу 
                if (!TH.CheckItemsToBasket(Chrome))
                {
                    Console.WriteLine("Не соответствие корзины товарам в  заказе");
                    Assert.IsTrue(false);
                }
                else
                {
                    Console.WriteLine("Товары в корзине соответствуют товарам в заказе");                    
                }
            }
            catch
            {
                Console.WriteLine("Не удалось проверить товары в заказе");
                Assert.IsTrue(false);
            }
            Chrome.Navigate().Refresh();
            try
            {   // Ищем блок с баллами                
                Chrome.FindElement(By.CssSelector(".bonus-help")).Click();
                if (Chrome.FindElement(By.CssSelector(".bonus-help__popover")).Text == "") 
                {
                    Assert.IsTrue(false);
                }
                else
                {
                    Console.WriteLine("Поповер виден, регресса нет");
                }
                
            }
            catch
            {
                Console.WriteLine("Не отображается поповер, регресс \nДефект: https://trello.com/c/joiW3mcq");
                Assert.IsTrue(false);
            }
        }

        [DataTestMethod]     // Отрицательные тесты, если вариант принимается, тест падает 
        [DataRow("")]        // 0 символов
        [DataRow("     ")]  // 5 пробелов
        [DataRow("t     ")] // Символ и 5 пробелов
        [DataRow("test")]    // 4 символа        
        [DataRow("te s t")]  // Тест с пробелами
        [DataRow("testtesttes")]// Тест на 11 символов
        [DataRow("кириллица")]  // Тест на кириллицу
        [DataRow("$%)(|}{![]")]  // Cимволы
        public void RegistrationPasswordTest(string pass)  // Дефект https://trello.com/c/zxwOXLc2
        {
            try
            {   // Инициализируем
                TH.DefaultStation(Chrome, baseURL);
                Console.WriteLine("Корректная инициализация драйвера");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера");
                Assert.IsTrue(false);
            }
            try
            {   // Открываем попап регистрации
                TH.OpenRegistration(Chrome);
            }
            catch
            {
                Console.WriteLine("Не удается открыть попап регистрации");
                Assert.IsTrue(false);
            }
            try
            {   // Заполняем данные корректно (для срабатывания контролей на поле "Пароль")
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[2].SendKeys("test");
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[3].SendKeys("79990000000");
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[4].SendKeys("22/12/1996");
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[5].SendKeys("test@te.st");
                // Заполняем поле пароля
                Chrome.FindElements(By.CssSelector(".mb15 .input-lg"))[2].SendKeys(pass);                
            }
            catch
            {
                Console.WriteLine("Не удается заполнить поля формы регистрации");
                Assert.IsTrue(false);
            }
            try
            {
                Chrome.FindElement(By.CssSelector(".js-sms-confirm .text-uppercase")).Click();
                System.Threading.Thread.Sleep(sleeper);
                if (Chrome.FindElement(By.CssSelector(".form-error .mb5")).Text.Contains("Длина пароля должна быть не менее 5 и не более 10 символов"))
                {
                    Console.WriteLine("Контроль корректно сработал со значением \""+ pass+"\"");
                }  
                else
                if (Chrome.FindElement(By.CssSelector(".form-error .mb5")).Text.Contains("Использование русских букв недопустимо"))
                {
                    Console.WriteLine("Контроль корректно сработал со значением \"" + pass + "\"");
                }
                else
                {
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                if (Chrome.FindElement(By.CssSelector(".col-xs-5")).Displayed)
                {
                    Console.WriteLine("Контроль не сработал со значением \"" + pass + "\" Значение принялось, как корректное.\nРегресс произошел, дефект: https://trello.com/c/zxwOXLc2");
                }
                else
                {
                    Console.WriteLine("Контроль не сработал со значением \"" + pass + "\"\nРегресс произошел, дефект: https://trello.com/c/zxwOXLc2");
                }
                Assert.IsTrue(false);
            }
        }

        [DataTestMethod]     // Отрицательные тесты, если вариант принимается, тест падает
        [DataRow("")]
        [DataRow("     ")]
        [DataRow("  @   ")]  
        [DataRow("t  @.   ")] 
        [DataRow("te@st")]         
        [DataRow("te s t@")]  
        [DataRow("@testtesttes")]
        [DataRow("кири@.ллица")]  
        [DataRow("@.")]
        [DataRow("bk.ru")]
        [DataRow("bk.ru@")]
        [DataRow("$%)(|}{![]")]  // Cимволы
        public void RegistrationEmailTest(string email)  // Дефект https://trello.com/c/MGX4sV4S
        {
            try
            {   // Инициализируем
                TH.DefaultStation(Chrome, baseURL);
                Console.WriteLine("Корректная инициализация драйвера");
            }
            catch
            {
                Console.WriteLine("Не корректная инициализация драйвера");
                Assert.IsTrue(false);
            }
            try
            {   // Открываем попап регистрации
                TH.OpenRegistration(Chrome);
            }
            catch
            {
                Console.WriteLine("Не удается открыть попап регистрации");
                Assert.IsTrue(false);
            }
            try
            {   // Заполняем данные корректно (для срабатывания контролей на поле "e-mail")
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[2].SendKeys("test");
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[3].SendKeys("79990000000");
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[4].SendKeys("22/12/1996");
                Chrome.FindElements(By.CssSelector(".mb15 .input-lg"))[2].SendKeys("testtest");
                // Заполняем поле почты
                Chrome.FindElements(By.CssSelector(".mb5 input.input-lg"))[5].SendKeys(email);
            }
            catch
            {
                Console.WriteLine("Не удается заполнить поля формы регистрации");
                Assert.IsTrue(false);
            }
            try
            {
                Chrome.FindElement(By.CssSelector(".js-sms-confirm .text-uppercase")).Click();
                System.Threading.Thread.Sleep(sleeper);
                if (Chrome.FindElement(By.CssSelector(".form-error .mb5")).Text.Contains("E-mail указан неверно"))
                {
                    Console.WriteLine("Контроль корректно сработал со значением \"" + email + "\"");
                }
                else                
                {
                    Assert.IsTrue(false);
                }
            }
            catch
            {
                if (Chrome.FindElement(By.CssSelector(".col-xs-5")).Displayed)
                {
                    Console.WriteLine("Контроль не сработал при со значением \"" + email + "\" Значение принялось, как корректное.\nРегресс произошел, дефект: https://trello.com/c/MGX4sV4S");
                }
                else
                {
                    Console.WriteLine("Контроль не сработал при со значением \"" + email + "\"\nРегресс произошел, дефект: https://trello.com/c/MGX4sV4S");
                }
                Assert.IsTrue(false);
            }
        }
        
        [TestMethod]
        public void CloseRegistrationPopap() // Доделать 

        {
            TH.DefaultStation(Chrome,baseURL);
            Chrome.FindElement(By.CssSelector(".header-auth__link .b")).Click();
            Chrome.FindElement(By.CssSelector(".fancybox-slide")).Click();
        }


        //[TestMethod]
        public void test() // Тест Функций 
        {
            
        }
    } 
}
/*
 
 */