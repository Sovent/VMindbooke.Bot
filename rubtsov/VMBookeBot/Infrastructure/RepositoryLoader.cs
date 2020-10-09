using System;
using System.Collections.Generic;
using VMBookeBot.Domain;

namespace VMBookeBot.Infrastructure
{
    //стал использовать singleton т.к. не знал как еще передать в пустой конструктор BackgroundServiceProvider'a 
    //созданный репозиторий. 
    public class RepositoryLoader
    {
        private static IUserActivityRepository _repository;

        public RepositoryLoader() {}
        
        public static IUserActivityRepository Load()
        {
            if (_repository == null)
            {
                _repository = new UserActivityRepository(
                    new List<int>(),
                    new List<Guid>(),
                    new List<int>());
            }
            return _repository;
        }
    }
}