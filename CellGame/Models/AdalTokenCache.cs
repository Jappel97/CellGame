using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CellGame.Models
{
    public class AdalTokenCache : TokenCache
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private string _userId;
        private UserTokenCache _cache;

        public AdalTokenCache(string signedInUserId)
        {
            // associate the cache to the current user of the web app
            _userId = signedInUserId;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            this.BeforeWrite = BeforeWriteNotification;
            // look up the entry in the database
            _cache = _db.UserTokenCacheList.FirstOrDefault(c => c.WebUserUniqueId == _userId);
            // place the entry in memory
            this.Deserialize((_cache == null) ? null : MachineKey.Unprotect(_cache.CacheBits,"ADALCache"));
        }

        // clean up the database
        public override void Clear()
        {
            base.Clear();
            var cacheEntry = _db.UserTokenCacheList.FirstOrDefault(c => c.WebUserUniqueId == _userId);
            _db.UserTokenCacheList.Remove(cacheEntry);
            _db.SaveChanges();
        }

        // Notification raised before ADAL accesses the cache.
        // This is your chance to update the in-memory copy from the DB, if the in-memory version is stale
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            if (_cache == null)
            {
                // first time access
                _cache = _db.UserTokenCacheList.FirstOrDefault(c => c.WebUserUniqueId == _userId);
            }
            else
            { 
                // retrieve last write from the DB
                var status = from e in _db.UserTokenCacheList
                             where (e.WebUserUniqueId == _userId)
                select new
                {
                    LastWrite = e.LastWrite
                };

                // if the in-memory copy is older than the persistent copy
                if (status.First().LastWrite > _cache.LastWrite)
                {
                    // read from from storage, update in-memory copy
                    _cache = _db.UserTokenCacheList.FirstOrDefault(c => c.WebUserUniqueId == _userId);
                }
            }
            this.Deserialize((_cache == null) ? null : MachineKey.Unprotect(_cache.CacheBits, "ADALCache"));
        }

        // Notification raised after ADAL accessed the cache.
        // If the HasStateChanged flag is set, ADAL changed the content of the cache
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if state changed
            if (this.HasStateChanged)
            {
                if (_cache == null)
                {
                    _cache = new UserTokenCache
                    {
                        WebUserUniqueId = _userId
                    };
                }

                _cache.CacheBits = MachineKey.Protect(this.Serialize(), "ADALCache");
                _cache.LastWrite = DateTime.Now;

                // update the DB and the lastwrite 
                _db.Entry(_cache).State = _cache.UserTokenCacheId == 0 ? EntityState.Added : EntityState.Modified;
                _db.SaveChanges();
                this.HasStateChanged = false;
            }
        }

        void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {
            // if you want to ensure that no concurrent write take place, use this notification to place a lock on the entry
        }

        public override void DeleteItem(TokenCacheItem item)
        {
            base.DeleteItem(item);
        }
    }
}
