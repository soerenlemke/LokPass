namespace LokPass.Core.Password.Crypto;

public sealed record UserConfiguration(
    byte[] MasterKey,
    byte[] Salt,
    DateTime CreatedAt
);

// TODO: public sealed record UserConfiguration()