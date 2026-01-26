using KellermanSoftware.CompareNetObjects;

namespace Application.services
{
    public class ObjectCompareService
    {
        private readonly CompareLogic _compareLogic;

        public ObjectCompareService()
        {
            _compareLogic = new CompareLogic(new ComparisonConfig
            {
                IgnoreCollectionOrder = true,
                CompareChildren = true,
                MaxDifferences = 50
            });
        }

        public ComparisonResult Compare<T>(T oldObject, T newObject)
        {
            return _compareLogic.Compare(oldObject, newObject);
        }
    }
}