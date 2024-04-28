using Microsoft.Playwright;

namespace BigEcommerceApp.Tests.Models {
  public class ContactUsPage : MainPage {
        private IPage _page;
        public ContactUsPage(IPage page) : base(page) { _page = page; }
    }
  }
