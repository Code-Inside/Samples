using System;

namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for a Live Tile service.
    /// </summary>
    public interface ILiveTileService
    {
        /// <summary>
        /// Pins a tile to start page.
        /// </summary>
        /// <param name="uri">The Url.</param>
        /// <param name="tileInfo">The tile information.</param>
        void PinToStart(Type viewModelType, TileInfo tileInfo);
    }
}
