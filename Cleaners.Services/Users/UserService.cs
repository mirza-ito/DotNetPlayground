﻿using Cleaners.Core.Domain;
using Cleaners.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cleaners.Services.Users
{
    /// <summary>
    /// User service interface implementation
    /// </summary>
    public class UserService : IUserService
    {
        #region Fields

        private readonly IRepository _repository;

        /// <summary>
        /// We will user already existing User methods defined in UserManager when appropriate
        /// </summary>
        private readonly UserManager<User> _userManager;

        #endregion Fields

        public UserService(IRepository repository, UserManager<User> userManager)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Generate token for email confirmation.
            // Same tokens are send to users(Part of confirmation URI) when confirmation is their responsibility
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Since we don' use IRepository.Create, we will set creation date manually
            user.CreationDateUtc = DateTime.UtcNow;
            // Email confirmation is only required when client requests for new account
            user.EmailConfirmed = true;

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Since we don' use IRepository.Update, we will set update date manually
            user.LastUpdateDateUtc = DateTime.UtcNow;

            return await _userManager.UpdateAsync(user);
        }

        public IEnumerable<User> Get()
        {
            return _userManager.Users.ToList();
        }

        public async Task<IEnumerable<User>> GetAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}