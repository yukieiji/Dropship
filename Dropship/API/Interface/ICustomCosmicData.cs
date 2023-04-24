namespace Dropship.API.Interface;

public interface ICustomCosmicData<T> where T : CosmeticData
{
    public const int DisplayOrder = 99;

    public string CollectionName { get; }
    public string Name { get; }

    public string Id { get; }

    public T GetData();
}
