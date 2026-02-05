using Microsoft.Playwright;

namespace EChamado.E2E.Tests.PageObjects;

/// <summary>
/// Page Object Model para a página de listagem de chamados
/// </summary>
public class OrderListPage
{
    private readonly IPage _page;

    public OrderListPage(IPage page)
    {
        _page = page;
    }

    // Seletores
    private const string NewOrderButton = "[data-test='new-order-button']";
    private const string SearchInput = "[data-test='search-input']";
    private const string SearchButton = "[data-test='search-button']";
    private const string FilterDropdown = "[data-test='filter-dropdown']";
    private const string OrdersTable = "[data-test='orders-table']";
    private const string OrderRow = "[data-test='order-row']";
    private const string PaginationNext = "[data-test='pagination-next']";
    private const string PaginationPrevious = "[data-test='pagination-previous']";
    private const string LoadingIndicator = "[data-test='loading']";

    // Ações
    public async Task ClickNewOrderAsync()
    {
        await _page.ClickAsync(NewOrderButton);
    }

    public async Task SearchAsync(string searchTerm)
    {
        await _page.FillAsync(SearchInput, searchTerm);
        await _page.ClickAsync(SearchButton);
        await WaitForLoadingToFinishAsync();
    }

    public async Task SelectFilterAsync(string filterValue)
    {
        await _page.SelectOptionAsync(FilterDropdown, filterValue);
        await WaitForLoadingToFinishAsync();
    }

    public async Task ClickOrderAsync(int orderIndex)
    {
        var orderRows = await _page.QuerySelectorAllAsync(OrderRow);
        if (orderIndex < orderRows.Count)
        {
            await orderRows[orderIndex].ClickAsync();
        }
    }

    public async Task GoToNextPageAsync()
    {
        if (await _page.IsEnabledAsync(PaginationNext))
        {
            await _page.ClickAsync(PaginationNext);
            await WaitForLoadingToFinishAsync();
        }
    }

    public async Task GoToPreviousPageAsync()
    {
        if (await _page.IsEnabledAsync(PaginationPrevious))
        {
            await _page.ClickAsync(PaginationPrevious);
            await WaitForLoadingToFinishAsync();
        }
    }

    // Verificações
    public async Task<int> GetOrderCountAsync()
    {
        var orderRows = await _page.QuerySelectorAllAsync(OrderRow);
        return orderRows.Count;
    }

    public async Task<bool> IsOrderVisibleAsync(string orderTitle)
    {
        return await _page.IsVisibleAsync($"text={orderTitle}");
    }

    public async Task<bool> IsNewOrderButtonVisibleAsync()
    {
        return await _page.IsVisibleAsync(NewOrderButton);
    }

    public async Task<bool> IsTableVisibleAsync()
    {
        return await _page.IsVisibleAsync(OrdersTable);
    }

    public async Task WaitForPageLoadAsync()
    {
        await _page.WaitForSelectorAsync(OrdersTable);
        await WaitForLoadingToFinishAsync();
    }

    private async Task WaitForLoadingToFinishAsync()
    {
        // Aguardar o indicador de loading desaparecer
        await _page.WaitForSelectorAsync(LoadingIndicator, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = 10000
        });
    }
}