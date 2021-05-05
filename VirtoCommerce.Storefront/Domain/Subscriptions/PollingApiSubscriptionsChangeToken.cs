using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Domain.Subscriptions
{
    public class PollingApiSubscriptionsChangeToken : IChangeToken
    {
        private readonly ISubscriptionModule _subscriptionApi;
        private static DateTime _previousChangeTimeUtcStatic;
        private static DateTime _lastCheckedTimeUtcStatic;
        private readonly DateTime _previousChangeTimeUtc;
        private readonly TimeSpan _pollingInterval;
        private static readonly object _lock = new object();

        static PollingApiSubscriptionsChangeToken()
        {
            _previousChangeTimeUtcStatic = _lastCheckedTimeUtcStatic = DateTime.UtcNow;
        }
        public PollingApiSubscriptionsChangeToken(ISubscriptionModule subscriptionApi, TimeSpan pollingInterval)
        {
            _pollingInterval = pollingInterval;
            _subscriptionApi = subscriptionApi;
            _previousChangeTimeUtc = _previousChangeTimeUtcStatic;
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                var currentTime = DateTime.UtcNow;
                if (currentTime - _lastCheckedTimeUtcStatic < _pollingInterval)
                {
                    return false;
                }

                //Need to prevent API flood for multiple token instances
                var lockTaken = Monitor.TryEnter(_lock);
                try
                {
                    if (lockTaken)
                    {
                        var result = _subscriptionApi.SearchSubscriptions(new AutoRestClients.SubscriptionModuleApi.Models.SubscriptionSearchCriteria
                        {
                            Skip = 0,
                            Take = int.MaxValue,
                            ResponseGroup = ((int)SubscriptionResponseGroup.Default).ToString(),
                            ModifiedSinceDate = _previousChangeTimeUtcStatic
                        });
                        if (result.TotalCount > 0)
                        {
                            _previousChangeTimeUtcStatic = currentTime;
                            foreach (var customerId in result.Subscriptions.Select(x => x.CustomerId))
                            {
                                SubscriptionCacheRegion.ExpireCustomerSubscription(customerId);
                            }
                        }
                        _lastCheckedTimeUtcStatic = currentTime;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(_lock);
                    }
                }
                //Always return false, do expiration only through direct SubscriptionCacheRegion.ExpireCustomerSubscription call
                return false;
            }
        }

        /// <summary>
        /// Does not actually register callbacks.
        /// </summary>
        /// <param name="callback">This parameter is ignored</param>
        /// <param name="state">This parameter is ignored</param>
        /// <returns>A disposable object that noops when disposed</returns>
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => EmptyDisposable.Instance;
    }
}
