﻿using System;
using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Services.Customers.Cache
{
    /// <summary>
    /// Customer cache event consumer (used for caching of customer password expiration)
    /// </summary>
    public partial class CustomerCacheEventConsumer :
        //settings
        IConsumer<EntityUpdated<Setting>>,
        //customer role
        IConsumer<EntityUpdated<CustomerRole>>,
        //passwords
        IConsumer<CustomerPasswordChangedEvent>
    {
        #region Constants

        /// <summary>
        /// Key for value whether customer password is expired
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public const string CUSTOMER_PASSWORD_EXPIRED = "Nop.customers.passwordisexpired-{0}";
        public const string CUSTOMER_PASSWORD_EXPIRED_PATTERN_KEY = "Nop.customers.passwordisexpired";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public CustomerCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        #endregion

        #region Methods

        //settings
        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //depends on CustomerSettings.PasswordLifetime
            if (eventMessage.Entity.Name.Equals("customersettings.passwordlifetime", StringComparison.InvariantCultureIgnoreCase))
                _cacheManager.RemoveByPattern(CUSTOMER_PASSWORD_EXPIRED_PATTERN_KEY);
        }

        //customer role
        public void HandleEvent(EntityUpdated<CustomerRole> eventMessage)
        {
            _cacheManager.RemoveByPattern(CUSTOMER_PASSWORD_EXPIRED_PATTERN_KEY);
        }

        //password changed
        public void HandleEvent(CustomerPasswordChangedEvent eventMessage)
        {
            _cacheManager.Remove(string.Format(CUSTOMER_PASSWORD_EXPIRED, eventMessage.Password.CustomerId));
        }

        #endregion
    }
}
