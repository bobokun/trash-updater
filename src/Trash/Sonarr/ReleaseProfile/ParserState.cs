﻿using System;
using System.Collections.Generic;
using Serilog;
using Trash.Extensions;

namespace Trash.Sonarr.ReleaseProfile
{
    public enum TermCategory
    {
        Required,
        Ignored,
        Preferred
    }


    public class ParserState
    {
        public ParserState(ILogger logger)
        {
            Log = logger;
            ResetParserState();
        }

        private ILogger Log { get; }
        public string? ProfileName { get; set; }
        public int? Score { get; set; }
        public ScopedState<TermCategory?> CurrentCategory { get; } = new();
        public bool InsideCodeBlock { get; set; }
        public int ProfileHeaderDepth { get; set; }
        public int CurrentHeaderDepth { get; set; }
        public int LineNumber { get; set; }
        public IDictionary<string, ProfileData> Results { get; } = new Dictionary<string, ProfileData>();

        // If null, then terms are not considered optional
        public ScopedState<bool> TermsAreOptional { get; } = new();

        public bool IsValid => ProfileName != null && CurrentCategory.Value != null &&
                               // If category is preferred, we also require a score
                               (CurrentCategory.Value != TermCategory.Preferred || Score != null);

        public List<string> IgnoredTerms
            => TermsAreOptional.Value ? Profile.Optional.Ignored : Profile.Ignored;

        public List<string> RequiredTerms
            => TermsAreOptional.Value ? Profile.Optional.Required : Profile.Required;

        public Dictionary<int, List<string>> PreferredTerms
            => TermsAreOptional.Value ? Profile.Optional.Preferred : Profile.Preferred;

        public ProfileData Profile
        {
            get
            {
                if (ProfileName == null)
                {
                    throw new NullReferenceException();
                }

                return Results.GetOrCreate(ProfileName);
            }
        }

        public void ResetParserState()
        {
            ProfileName = null;
            Score = null;
            InsideCodeBlock = false;
            ProfileHeaderDepth = -1;
        }

        public void ResetScopeState(int scope)
        {
            if (CurrentCategory.Reset(scope))
            {
                Log.Debug("  - Reset Category State for Scope: {Scope}", scope);
            }

            if (TermsAreOptional.Reset(scope))
            {
                Log.Debug("  - Reset Optional State for Scope: {Scope}", scope);
            }
        }
    }
}
