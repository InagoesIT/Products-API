namespace Products_API.Helpers;

public class ResultOfEntity<TEntity>
{
    public TEntity Entity { get; private set; }
    public string Error { get; private set; }
    public bool IsSuccess { get; private set; }

    private ResultOfEntity() { }

    public static ResultOfEntity<TEntity> Success(TEntity entity)
    {
        return new ResultOfEntity<TEntity>
        {
            Entity = entity,
            IsSuccess = true
        };
    }

    public static ResultOfEntity<TEntity> Failure(string error)
    {
        return new ResultOfEntity<TEntity>
        {
            Error = error,
            IsSuccess = false
        };
    }
}