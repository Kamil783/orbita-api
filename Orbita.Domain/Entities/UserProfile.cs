using Orbita.Domain.Exceptions;
using Orbita.Domain.Helpers;
using Orbita.Domain.Helpers.Enums;

namespace Orbita.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public byte[]? AvatarData { get; init; }
    public string? AvatarContentType { get; init; }
    public int AvatarVersion { get; init; }

    public const int MaxAvatarBytes = 5 * 1024 * 1024;

    public UserProfile SetAvatar(byte[] avatarBytes, string? declaredContentType)
    {
        if (avatarBytes is null)
            throw new DomainValidationException("Avatar bytes are required.");

        if (avatarBytes.Length == 0)
            throw new DomainValidationException("Avatar file is empty.");

        if (avatarBytes.Length > MaxAvatarBytes)
            throw new DomainValidationException($"Avatar is too large. Max {MaxAvatarBytes} bytes.");

        var detected = ImageFormatDetector.Detect(avatarBytes);
        if (detected is null)
            throw new DomainValidationException("Unsupported image format. Allowed: JPEG, PNG, WebP.");

        if (!string.IsNullOrWhiteSpace(declaredContentType) && !ContentTypePolicy.IsAllowed(declaredContentType))
            throw new DomainValidationException("Unsupported content type. Allowed: image/jpeg, image/png, image/webp.");
        

        var normalizedContentType = detected.Value switch
        {
            ImageFormat.Jpeg => "image/jpeg",
            ImageFormat.Png => "image/png",
            ImageFormat.WebP => "image/webp",
            _ => throw new DomainValidationException("Unsupported image format.")
        };

        if (ByteArrayEquals(AvatarData, avatarBytes) &&
            string.Equals(AvatarContentType, normalizedContentType, StringComparison.OrdinalIgnoreCase))
        {
            return this;
        }

        var storedBytes = (byte[])avatarBytes.Clone();

        return new UserProfile
        {
            UserId = this.UserId,
            Name = this.Name,

            AvatarData = storedBytes,
            AvatarContentType = normalizedContentType,
            AvatarVersion = checked(this.AvatarVersion + 1)
        };
    }

    public UserProfile RemoveAvatar()
    {
        if (AvatarData is null && AvatarContentType is null)
            return this;

        return new UserProfile
        {
            UserId = this.UserId,
            Name = this.Name,

            AvatarData = null,
            AvatarContentType = null,
            AvatarVersion = checked(this.AvatarVersion + 1)
        };
    }

    private static bool ByteArrayEquals(byte[]? a, byte[]? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        if (a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;

        return true;
    }
}