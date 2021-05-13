using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Pricing;
using VirtoCommerce.Storefront.Model.Pricing.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class PricingWorkContextBuilderExtensions
    {
        public static Task WithPricelistsAsync(this IWorkContextBuilder builder, IMutablePagedList<Pricelist> pricelists)
        {
            builder.WorkContext.CurrentPricelists = pricelists;
            return Task.CompletedTask;
        }

        public static Task WithPricelistsAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var pricingService = serviceProvider.GetRequiredService<IPricingService>();

            Func<int, int, IEnumerable<SortInfo>, IPagedList<Pricelist>> factory = (pageNumber, pageSize, sortInfos) =>
            {
                IList<Pricelist> pricelists = new List<Pricelist>();
                //Do not evaluate price lists for anonymous user
                if (builder.WorkContext.CurrentUser.IsRegisteredUser)
                {
					var priceListEvaluationContext = builder.WorkContext.ToPriceEvaluationContext(null);
					priceListEvaluationContext.CertainDate = DateTime.UtcNow;
					pricelists = pricingService.EvaluatePricesLists(priceListEvaluationContext, builder.WorkContext);
                }
                return new StaticPagedList<Pricelist>(pricelists, pageNumber, pageSize, pricelists.Count);
            };
            return builder.WithPricelistsAsync(new MutablePagedList<Pricelist>(factory, 1, int.MaxValue));
        }
    }
}
