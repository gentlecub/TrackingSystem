using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Data;
using TrackingSystem.App.Models;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u =>
                u.Username == username &&
                u.Password == password);

        if (user != null)
        {
            _currentUser = user;
            return true;
        }

        return false;
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public bool HasRole(UserRole requiredRole)
    {
        if (_currentUser == null) return false;

        // Admin can do everything
        if (_currentUser.Role == UserRole.Admin) return true;

        // Manager can do manager and employee tasks
        if (_currentUser.Role == UserRole.Manager && requiredRole != UserRole.Admin)
            return true;

        // Employee can only do employee tasks
        return _currentUser.Role == requiredRole;
    }

    public bool IsAdmin() => _currentUser?.Role == UserRole.Admin;
    public bool IsManager() => _currentUser?.Role == UserRole.Manager || IsAdmin();
}
