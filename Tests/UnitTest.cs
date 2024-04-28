using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace BigEcommerceApp.Tests.Models;

[AllureNUnit]
public class UnitTest : PlaywrightTest {
  protected IBrowser browser {
    get;
    set;
  }
  protected IBrowserContext context {
    get;
    set;
  }

  public IPage page {
    get;
    set;
  }
  
  [SetUp]
  public async Task TestStart() {
    await ContextCreation();
  }

  [TearDown]
  public async Task TestClose() {
    await context !.CloseAsync();
    await browser !.CloseAsync();
  }

  public async Task ContextCreation() {
    BrowserNewContextOptions browserContext = new BrowserNewContextOptions();
    browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions{Headless = false});
    context = await browser.NewContextAsync(browserContext);
    page = await context.NewPageAsync();
  }

  [Test]
  [AllureOwner("Platon Ionov")]
  public async Task Case_0() {
    Console.WriteLine("---------------------------------------");
    Console.WriteLine("Запуск Case_0\n");
    MainPage mainPage = new MainPage(page);
    try {
      // Переход на страницу
      await mainPage.GoTo();

      // Проверка видимости раздела с товарами
      Assert.That(await page.Locator("//section[@id='guzhy']").IsVisibleAsync());
      Console.WriteLine("Товары отображаются");

      // Проверка количества элементов
      var numOfElems = await mainPage.GetNumberOfElements();
      Console.WriteLine("Количество = " + numOfElems);
      Assert.That(numOfElems, Is.EqualTo(28));

      // Проверка на то, что регион = Москва
      var textContent = await mainPage.GetRegion();
      Console.WriteLine("Регион = " + textContent);
      Assert.That(textContent, Is.EqualTo("Москва"));
    } catch (Exception ex) {
      Console.WriteLine($"Тест завершился с ошибкой: {ex.Message}");
      throw;
    } finally {
      await mainPage.GetScreenshot("screenshotCase_0"); // Делаем скриншот страницы
    }
  }

  [Test]
  [AllureOwner("Platon Ionov")]
  public async Task Case_1() {
    Console.WriteLine("\n---------------------------------------");
    Console.WriteLine("Запуск Case_1");
    MainPage mainPage = new MainPage(page);
    try {

      await mainPage.GoTo();

      // Выбор случайной пиццы
      await mainPage.TapRandomPizza();

      // Проверка соответствия наименований пиццы в главном меню и во всплывающем окне
      var pizzaNameOnMain = await mainPage.PizzaNameOnMain();
      var pizzaNameOnPopUp = await mainPage.PizzaNameOnPopUp();
      Assert.That(pizzaNameOnMain.Trim(), Is.EqualTo(pizzaNameOnPopUp), "Наименования не соответствуют");
      Console.WriteLine("Наименование в главном меню: " + pizzaNameOnMain +
                        "\nНаименование в окне: " + pizzaNameOnPopUp + "\nНаименования соответствуют\n");

      // Проверка, что цены на главной странице и во всплывающем окне различны
      var priceOnMain = await mainPage.PriceOnMain();
      string resultString = priceOnMain.Substring(3, 5);
      var priceOnPopUp = await mainPage.PriceOnPopUp();
      Assert.That(resultString !.Trim(), !Is.EqualTo(priceOnPopUp), "Цены одинаковые");
      Console.WriteLine("Цена в главном меню: " + resultString + "\nЦена в окне: " + priceOnPopUp + " ₽" +
                        "\nЦены не совпадают\n");

      // Выбор размера пиццы, добавление в корзину и указание адреса
      await mainPage.SizeAndOrder();
      await mainPage.AddressSelection();

      // Проверка количества пицц в корзине
      var amountOfPizzas = await mainPage.AmountOfPizzas();
      Assert.That(
          await page.Locator("//div[contains(@class, 'grid')]//div[@data-testid='cart-button__quantity']")
              .IsVisibleAsync(),
          "Счетчик не появился");

      // Проверка на то, что кол-во пицц в корзине = 1
      Assert.That(amountOfPizzas, Is.EqualTo("1"), "Значение != 1");
      Console.WriteLine("Счетчик появился, значение = " + amountOfPizzas);
    } catch (Exception ex) {
      Console.WriteLine($"Тест завершился с ошибкой: {ex.Message}");
      throw;
    } finally {
      await mainPage.GetScreenshot("screenshotCase_1"); // Делаем скриншот страницы
    }
  }

  [Test]
  [AllureOwner("Platon Ionov")]
  public async Task Case_2() {
    Console.WriteLine("\n---------------------------------------");
    Console.WriteLine("Запуск Case_2");
    MainPage mainPage = new MainPage(page);
    try {

      await mainPage.GoTo();
      
      // Выбор случайной пиццы, ее размера, добавление в корзину и указание адреса
      await mainPage.TapRandomPizza();
      await mainPage.SizeAndOrder();
      await mainPage.AddressSelection();

      // Добавление 5 пицц в корзину
      List<string> pizzaNamesOnMain = new List<string>();

      var pizzaNameOnMainF = await mainPage.PizzaNameOnMain();
      pizzaNamesOnMain.Add(pizzaNameOnMainF);
      Console.WriteLine("Наименование: " + pizzaNameOnMainF);

      for (int i = 0; i < 4; i++) {
        await mainPage.TapRandomPizza();
        await mainPage.SizeAndOrder();
        var pizzaNameOnMain = await mainPage.PizzaNameOnMain();
        pizzaNamesOnMain.Add(pizzaNameOnMain);
        Console.WriteLine("Наименование: " + pizzaNameOnMain);
      }
      
      // Проверка на то, что кол-во пицц в корзине = 5
      await page.WaitForTimeoutAsync(1000); // Нужно во избежание отработки AmountOfPizzas() быстрее, чем счетчик обновится
      var amountOfPizzas = await mainPage.AmountOfPizzas();
      Assert.That(
          await page.Locator("//div[contains(@class, 'grid')]//div[@data-testid='cart-button__quantity']")
              .IsVisibleAsync(),
          "Счетчик не появился");
      Assert.That(amountOfPizzas, Is.EqualTo("5"), "Значение != 5");
      Console.WriteLine("\nЗначение = " + amountOfPizzas + "\n");

      // Нажатие на кнопку корзины и получение наименований пицц оттуда для сравнения
      await mainPage.CartClick();
      List<string> pizzaNamesOnCart = new List<string>();

      for (int j = 1; j < 6; j++) {
        var pizzaNameOnCart = await mainPage.PizzaNameOnCart(j);
        pizzaNamesOnCart.Add(pizzaNameOnCart);
        Console.WriteLine("Наименование в корзине: " + pizzaNameOnCart);
      }

      // Сравнение наименований пицц на главной странице и в корзине
      for (int k = 0; k < pizzaNamesOnMain.Count; k++) {
        Assert.That(pizzaNamesOnMain[k].Trim(),
                    Is.EqualTo(pizzaNamesOnCart[k].Trim()), "Наименования не соответствуют");
      }
      Console.WriteLine("\nНаименования соответствуют");

      // Получение итоговой стоимости
      var priceFull = await mainPage.GetPrice();
      Console.WriteLine("\nИтоговая стоимость: " + priceFull);
    } catch (Exception ex) {
      Console.WriteLine($"Тест завершился с ошибкой: {ex.Message}");
      throw;
    } finally {
      await mainPage.GetScreenshot("screenshotCase_2"); // Делаем скриншот страницы
    }
  }

  [Test]
  [AllureOwner("Platon Ionov")]
  public async Task Case_3() {
    Console.WriteLine("\n---------------------------------------");
    Console.WriteLine("Запуск Case_3\n");
    MainPage mainPage = new MainPage(page);
    try {

      await mainPage.GoTo();

      // Переход в раздел "Контакты"
      await mainPage.ContactUsClick();

      ContactUsPage contactUsPagePage = new ContactUsPage(page);

      // Получение номера телефона и почты
      var phoneNumber = await contactUsPagePage.GetPhoneNumber();
      var mail = await contactUsPagePage.GetMail();

      // Проверка на то, что номер телефона и почта верны и отображаются
      Assert.That(phoneNumber, Is.EqualTo("8 800 302-00-60"));
      Console.WriteLine("Номер телефона: " + phoneNumber);
      Assert.That(mail, Is.EqualTo("feedback@dodopizza.com"));
      Console.WriteLine("Почта: " + mail);
      Console.WriteLine("\nНомер телефона и почта верны и отображаются");
    } catch (Exception ex) {
      Console.WriteLine($"Тест завершился с ошибкой: {ex.Message}");
      throw;
    } finally {
      await mainPage.GetScreenshot("screenshotCase_3"); // Делаем скриншот страницы
    }
  }
}
