# ExtraChat

ExtraChat is a Dalamud plugin and an associated server that function
to add extra chat channels to FFXIV. Basically, this adds
cross-data-centre linkshells that don't have a member limit.

## Security and privacy

For your privacy and to ensure a lack of liability for server hosts,
messages and linkshell names are encrypted between the members of each
linkshell. The server is unable to decrypt the content of messages or
linkshell names.

The server *does* know which characters are in which linkshells (an
operational requirement).

Due to this design decision, it is impossible for a server to moderate
these extra linkshells, and linkshells will need to self-moderate
instead. As such, there is no ability to report users.

## Encryption details

When a user initiates the process to create a linkshell, their client
generates a random shared secret. The secret is saved locally by the
client. When the user invites another user to the linkshell, a
Diffie-Hellman key exchange is mediated by the server between the two
users, and then the inviter transmits the shared secret to the
invitee, encrypting it with their ephemeral shared secret created by
the key exchange. Due to the nature of the Diffie-Hellman exchange,
the server is unable to read the shared secret when it is sent.

After this, the newly-invited user receives information about the
linkshell they have been invited to, and can decrypt the name, as well
as see any members. If the invitee decides to join, their client will
save this shared secret.

Any messages sent to the linkshell are encrypted with the shared
secret, making their contents opaque to the server. The only way to
read these messages is to know the shared secret, which the server is
never able to discern.
