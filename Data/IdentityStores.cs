using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data;

public class IdentityStore(AppDbContext db) : IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
{
    public void Dispose() { }
    public Task<string> GetUserIdAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.Id);
    public Task<string?> GetUserNameAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.UserName);
    public Task SetUserNameAsync(ApplicationUser u, string? n, CancellationToken c) { u.UserName = n; return Task.CompletedTask; }
    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.NormalizedUserName);
    public Task SetNormalizedUserNameAsync(ApplicationUser u, string? n, CancellationToken c) { u.NormalizedUserName = n; return Task.CompletedTask; }
    public async Task<IdentityResult> CreateAsync(ApplicationUser u, CancellationToken c) { db.Users.Add(u); return await Save(c); }
    public async Task<IdentityResult> UpdateAsync(ApplicationUser u, CancellationToken c) { db.Users.Update(u); return await Save(c); }
    public async Task<IdentityResult> DeleteAsync(ApplicationUser u, CancellationToken c) { db.Users.Remove(u); return await Save(c); }
    public Task<ApplicationUser?> FindByIdAsync(string id, CancellationToken c) => db.Users.FirstOrDefaultAsync(x => x.Id == id, c);
    public Task<ApplicationUser?> FindByNameAsync(string n, CancellationToken c) => db.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == n, c);
    public Task SetPasswordHashAsync(ApplicationUser u, string? h, CancellationToken c) { u.PasswordHash = h; return Task.CompletedTask; }
    public Task<string?> GetPasswordHashAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.PasswordHash);
    public Task<bool> HasPasswordAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.PasswordHash is not null);
    public Task SetEmailAsync(ApplicationUser u, string? e, CancellationToken c) { u.Email = e; return Task.CompletedTask; }
    public Task<string?> GetEmailAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.Email);
    public Task<bool> GetEmailConfirmedAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.EmailConfirmed);
    public Task SetEmailConfirmedAsync(ApplicationUser u, bool v, CancellationToken c) { u.EmailConfirmed = v; return Task.CompletedTask; }
    public Task<ApplicationUser?> FindByEmailAsync(string e, CancellationToken c) => db.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == e, c);
    public Task<string?> GetNormalizedEmailAsync(ApplicationUser u, CancellationToken c) => Task.FromResult(u.NormalizedEmail);
    public Task SetNormalizedEmailAsync(ApplicationUser u, string? e, CancellationToken c) { u.NormalizedEmail = e; return Task.CompletedTask; }
    private async Task<IdentityResult> Save(CancellationToken c) { try { await db.SaveChangesAsync(c); return IdentityResult.Success; } catch (Exception e) { return IdentityResult.Failed(new IdentityError { Description = e.Message }); } }
}

public class RoleStore(AppDbContext db) : IRoleStore<IdentityRole>
{
    public void Dispose() { }
    public Task<string> GetRoleIdAsync(IdentityRole r, CancellationToken c) => Task.FromResult(r.Id);
    public Task<string?> GetRoleNameAsync(IdentityRole r, CancellationToken c) => Task.FromResult(r.Name);
    public Task SetRoleNameAsync(IdentityRole r, string? n, CancellationToken c) { r.Name = n; return Task.CompletedTask; }
    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole r, CancellationToken c) => Task.FromResult(r.NormalizedName);
    public Task SetNormalizedRoleNameAsync(IdentityRole r, string? n, CancellationToken c) { r.NormalizedName = n; return Task.CompletedTask; }
    public async Task<IdentityResult> CreateAsync(IdentityRole r, CancellationToken c) { db.Roles.Add(r); await db.SaveChangesAsync(c); return IdentityResult.Success; }
    public async Task<IdentityResult> UpdateAsync(IdentityRole r, CancellationToken c) { db.Roles.Update(r); await db.SaveChangesAsync(c); return IdentityResult.Success; }
    public async Task<IdentityResult> DeleteAsync(IdentityRole r, CancellationToken c) { db.Roles.Remove(r); await db.SaveChangesAsync(c); return IdentityResult.Success; }
    public Task<IdentityRole?> FindByIdAsync(string id, CancellationToken c) => db.Roles.FirstOrDefaultAsync(x => x.Id == id, c);
    public Task<IdentityRole?> FindByNameAsync(string n, CancellationToken c) => db.Roles.FirstOrDefaultAsync(x => x.NormalizedName == n, c);
}
