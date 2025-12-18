using System;

namespace OctoAwesome.Notifications;

/// <summary>
/// Holds multiple disposables to dispose together
/// </summary>
public class StableCompositeDisposable : IDisposable
{
    private readonly IDisposable[] disposables;

    private bool disposedValue;

    private StableCompositeDisposable(IDisposable[] disposables)
    {
        this.disposables = disposables;
    }

    /// <summary>
    /// Creates an <see cref="StableCompositeDisposable"/> with the disposables 
    /// </summary>
    /// <param name="disposable1">The item to dispose</param>
    /// <param name="disposable2">The other item to dispose</param>
    /// <returns><see cref="StableCompositeDisposable"/> as an IDisposable</returns>
    public static IDisposable Create(IDisposable disposable1, IDisposable disposable2)
    {
        return new StableCompositeDisposable([disposable1, disposable2]);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var item in disposables)
                {
                    item.Dispose();
                }
            }

            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}