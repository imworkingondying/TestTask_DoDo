C# + NUnit + Playwright + Allure.

---------------------------------------
Подробное описание для запуска тестов.

1. Установить .NET 8.0
https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.204-windows-x64-installer
После установки перезагрузить устройство

2. В PowerShell прописать Set-ExecutionPolicy Unrestricted -Scope Process, затем ввести Y

3. В PowerShell перейти в папку проекта и прописать dotnet restore

4. В PowerShell в папке проекта прописать dotnet build

5. В консоли в папке проекта прописать bin\Debug\net8.0\playwright.ps1 install

6. Для запуска тестов необходимо прописать dotnet test

7. Для запуска конкретного теста необходимо прописать dotnet test --filter Name~"название теста"

---------------------------------------
Примечание: 
Команда в шаге 2 настраивает политику выполнения, без нее на шаге 5 будет ошибка.
Подробнее в документации: https://learn.microsoft.com/ru-ru/powershell/module/microsoft.powershell.security/set-executionpolicy?view=powershell-7.4

Запускать тесты с настройкой Headless = false для браузера обязательно, поскольку иначе сайт ДоДо догадывается, что что-то не так, и отправляет на определенную страницу, откуда дальнейшее тестирование невозможно.
Скриншот такой страницы есть в папке Screenshots, название screenshotCaseFailure.png.

---------------------------------------
Для отображения отчета Allure в виде HTML-страницы необходимо установить allure-commandline.

1. Установить Node.js
https://nodejs.org/en

2. В PowerShell в стандартной директории(прим: C:\Users\imworkingondying) прописать npm install -g allure-commandline

3. В PowerShell в папке с проектом перейти в bin\Debug\net8.0 и прописать allure serve allure-results
