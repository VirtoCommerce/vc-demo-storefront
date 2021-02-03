using System.Linq;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class DemoCustomerOrderService : CustomerOrderService, IDemoCustomerOrderService
    {
        private readonly ICatalogService _catalogService;
        private readonly IDemoCatalogService _demoCatalogService;

        public DemoCustomerOrderService(IOrderModule orderApi, ICatalogService catalogService, IDemoCatalogService demoCatalogService,
            IWorkContextAccessor workContextAccessor) : base(orderApi, workContextAccessor)
        {
            _catalogService = catalogService;
            _demoCatalogService = demoCatalogService;
        }

        public override async Task<CustomerOrder> GetOrderByNumberAsync(string number)
        {
            var order = await base.GetOrderByNumberAsync(number);
            await LoadProductsAsync(order);
            return order;
        }


        public override async Task<CustomerOrder> GetOrderByIdAsync(string id)
        {
            var order = await base.GetOrderByIdAsync(id);
            await LoadProductsAsync(order);
            return order;
        }

        protected override async Task<IPagedList<CustomerOrder>> InnerSearchOrdersAsync(OrderSearchCriteria criteria, WorkContext workContext)
        {
            var ordersPagedList = await base.InnerSearchOrdersAsync(criteria, workContext);
            var orders = ordersPagedList.ToArray();
            await LoadProductsAsync(orders.ToArray());
            return new StaticPagedList<CustomerOrder>(ordersPagedList, ordersPagedList.PageNumber, ordersPagedList.PageSize, ordersPagedList.TotalItemCount);
        }

        public async Task LoadProductsAsync(params CustomerOrder[] orders)
        {
            var productIds = orders.SelectMany(o => o.Items.Select(i => i.ProductId).Concat(o.ConfiguredGroups.Select(c => c.ProductId))).ToArray();
            var products = (await _catalogService.GetProductsAsync(productIds, ItemResponseGroup.None)).ToDictionary(x => x.Id, x => x);

            foreach (var lineItem in orders.SelectMany(o => o.Items))
            {
                lineItem.Product = products[lineItem.ProductId];
            }

            foreach (var group in orders.SelectMany(x => x.ConfiguredGroups))
            {
                group.Product = products[group.ProductId];
            }
        }       
    }
}
