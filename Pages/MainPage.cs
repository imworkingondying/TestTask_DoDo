using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;

namespace BigEcommerceApp.Tests.Models {

  [AllureNUnit]
  public class MainPage {
    private IPage _page;

    public MainPage(IPage page) { _page = page; }

    [AllureStep("Переход на dodopizza.ru/moscow")]
    // Метод для перехода на главную страницу
    public async Task GoTo() {
      await _page.GotoAsync("https://dodopizza.ru/moscow",
                            new PageGotoOptions{WaitUntil = WaitUntilState.DOMContentLoaded});
    }

    [AllureStep("Получаем количество элементов на странице")]
    // Метод для получения количества элементов на странице
    public async Task<int> GetNumberOfElements() {
      return await _page.Locator("//section[@id='guzhy']//article").CountAsync();
    }

    [AllureStep("Получаем названия региона")]
    // Метод для получения названия региона
    public async Task<string> GetRegion() {
      var element = _page.Locator(
          "a.header__about-slogan-text.header__about-slogan-text_locality.header__about-slogan-text_link");
      var region = await element.TextContentAsync();
      return region !;
    }

    [AllureStep("Скриншот")]
    // Метод для создания скришота
    public async Task GetScreenshot(string name){
      await _page.ScreenshotAsync(new (){Path = $"../../../Screenshots/{name}.png"});
    }

    // Метод для генерации случайного числа
    private List<int> usedNumbers = new List<int>(); // Список используемых чисел 
    private int value;

    // Пояснение: первая и девятнадцатая позиции (на момент написания теста) кончились
    // и вместо кнопки "Выбрать" отображается текст "Будет позже". Позиция два не является пиццей,
    // там конструктор и вместо кнопки "Выбрать" есть только кнопка "Собрать"
    public int GetRandomNumber() {
      Random random = new Random();
      do {
        value = random.Next(1, 29);
      } while (value == 1 | value == 2 | value == 19 | usedNumbers.Contains(value));
      usedNumbers.Add(value);
      return value;
    }

    [AllureStep("Выбираем случайную пиццу...")]
    // Метод для выбора случайной пиццы
    public async Task TapRandomPizza() {
      int value = GetRandomNumber();
      var tovar = _page.Locator($"//section[@id='guzhy']//article[{value}]/footer/button[text()='Выбрать']");
      await _page.WaitForTimeoutAsync(500);
      await tovar.ClickAsync();
      Console.WriteLine("\nВыбрана пицца под номером " + value);
    }

    [AllureStep("Получение названия пиццы с главной страницы")]
    // Метод для получения названия пиццы на главной странице
    public async Task<string> PizzaNameOnMain() {
      var textMain = _page.Locator($"//section[@id='guzhy']/article[{value}]/main/div/a");
      var pizzaNameOnMain = await textMain.TextContentAsync();
      return pizzaNameOnMain !;
    }

    [AllureStep("Получение названия пиццы со всплывающего окна")]
    // Метод для получения названия пиццы во всплывающем окне
    public async Task<string> PizzaNameOnPopUp() {
      var textPopUp = _page.Locator("//div[@class='popup-inner undefined']//h1");
      var pizzaNameOnPopUp = await textPopUp.TextContentAsync();
      return pizzaNameOnPopUp !;
    }

    [AllureStep("Получение цены пиццы на главной страницы")]
    // Метод для получения цены пиццы на главной странице
    public async Task<string> PriceOnMain() {
      var valueMain = _page.Locator($"//section[@id='guzhy']/article[{value}]/footer/div[1]");
      var priceOnMain = await valueMain.TextContentAsync();
      return priceOnMain !;
    }

    [AllureStep("Получение цены пиццы во всплывающем окне")]
    // Метод для получения цены во всплывающем окне
    public async Task<string> PriceOnPopUp() {
      var valuePopUp = _page.Locator(
          "//div[@class='popup-inner undefined']//button[contains(text(), 'В корзину')]//span[@class='money__value']");
      var priceOnPopUp = await valuePopUp.TextContentAsync();
      return priceOnPopUp !;
    }

    [AllureStep("Выбор размера пиццы и добавление в корзину")]
    // Метод для выбора размера пиццы и добавления в корзину
    public async Task SizeAndOrder() {
      await _page.Locator("//label[@data-testid='menu__pizza_size_Маленькая']").ClickAsync();
      await _page
          .Locator("//div[@class='popup-inner undefined']//div/button[@data-testid='button_add_to_cart']")
          .ClickAsync();
    }
    
    [AllureStep("Указание адреса")]
    // Метод для указания адреса(первого в списке)
    public async Task AddressSelection() {
      await _page
          .Locator(
              "//div[@class='popup-inner undefined']//div/button[@data-testid='how_to_get_order_pickup_action']")
          .ClickAsync();
      await _page
          .Locator("//div[@class='popup-inner undefined']//div/button[@data-testid='menu_product_select']")
          .ClickAsync();
    }

    [AllureStep("Получение количества пицц на кнопке корзины")]
    // Метод для получения количества пицц на кнопке корзины
    public async Task<string> AmountOfPizzas() {
      var amount =
          _page.Locator("//div[contains(@class, 'grid')]//div[@data-testid='cart-button__quantity']");
      var amountOfPizzas = await amount.TextContentAsync();
      return amountOfPizzas !;
    }

    [AllureStep("Нажатие на кнопку корзины")]
    // Метод для клика по корзине
    public async Task CartClick() {
      await _page.Locator("//div[contains(@class, 'grid')]/div[2]/button").ClickAsync();
    }

    [AllureStep("Получение названия пиццы в корзине")]
    // Метод для получения названия пиццы в корзине
    public async Task<string> PizzaNameOnCart(int value) {
      var imageAlt = $"(//picture[@data-type='product']//img)[{value}]";
      var pizzaNameOnCart = await _page.EvalOnSelectorAsync<string>(imageAlt, "img => img.alt");
      return pizzaNameOnCart;
    }

    [AllureStep("Получение итоговой цены")]
    // Метод для получения итоговой цены
    public async Task<string> GetPrice() {
      var priceCart = _page.Locator("//div[@class='price']");
      var price = await priceCart.TextContentAsync();
      return price !;
    }

    [AllureStep("Переход в раздел Контакты")]
    // Метод для клика по разделу "Контакты"
    public async Task ContactUsClick() {
      await _page.Locator("//div[@id='react-app']").GetByText("Контакты").ClickAsync();
    }

    [AllureStep("Получение номера телефона")]
    // Метод для получения номера телефона
    public async Task<string> GetPhoneNumber() {
      var textNumber = _page.Locator("(//a[@class='contacts-pizzerias__information-desc'])[1]");
      var phoneNumber = await textNumber.TextContentAsync();
      return phoneNumber !;
    }

    [AllureStep("Получение почты")]
    // Метод для получения почты
    public async Task<string> GetMail() {
      var textMail = _page.Locator("(//a[@class='contacts-pizzerias__information-desc'])[2]");
      var mail = await textMail.TextContentAsync();
      return mail !;
    }
  }
}
