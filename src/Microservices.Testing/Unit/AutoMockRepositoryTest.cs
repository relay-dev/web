using Arch.EntityFrameworkCore.UnitOfWork;
using Core.Plugins.NUnit.Unit;
using System;
using System.Collections.Generic;

namespace Microservices.Testing.Unit
{
    public class AutoMockRepositoryTest<TCUT> : AutoMockTest<TCUT> where TCUT : class
    {
        public override void Setup()
        {
            base.Setup();

            CurrentTestProperties.Set(RepositoryKey, new Dictionary<string, object>());
        }

        public void InjectTestData<TToMock>(List<TToMock> entities) where TToMock : class
        {
            var fakeRepository = new FakeRepository<TToMock>(entities);

            ResolveMock<IUnitOfWork>()
                .Setup(uow => uow.GetRepository<TToMock>(false))
                .Returns(fakeRepository);

            var fakeRepositories = (Dictionary<string, object>)CurrentTestProperties.Get(RepositoryKey);

            fakeRepositories.Add(typeof(TToMock).FullName, fakeRepository);

            CurrentTestProperties.Set(RepositoryKey, fakeRepositories);
        }

        public List<TToMock> GetTestData<TToMock>() where TToMock : class
        {
            var fakeRepositories = (Dictionary<string, object>)CurrentTestProperties.Get(RepositoryKey);

            string key = typeof(TToMock).FullName;

            if (!fakeRepositories.ContainsKey(key))
                throw new Exception("Cannot call GetTestData() without calling InitTestData() or InjectTestData() with the same generic type");

            var fakeRepository = (FakeRepository<TToMock>)fakeRepositories[key];

            return fakeRepository.TestData;
        }

        public void InitTestData<TToMock>() where TToMock : class
        {
            InjectTestData(new List<TToMock>());
        }

        private const string RepositoryKey = "_repository";
    }
}
