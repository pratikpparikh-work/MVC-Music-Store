namespace MvcMusicStore.Domain.Entities
{
    /// <summary>
    /// Represents a music artist.
    /// </summary>
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
