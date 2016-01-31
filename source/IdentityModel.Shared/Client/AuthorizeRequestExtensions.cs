﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IdentityModel.Client
{
    public static class AuthorizeRequestExtensions
    {
        public static string Create(this AuthorizeRequest request, object values)
        {
            return request.Create(ObjectToDictionary(values));
        }

        public static string CreateAuthorizeUrl(this AuthorizeRequest request,
            string clientId,
            string responseType,
            string scope = null,
            string redirectUri = null,
            string state = null,
            string nonce = null,
            string loginHint = null,
            string acrValues = null,
            string responseMode = null,
            object extra = null)
        {
            var values = new Dictionary<string, string>
            {
                { OAuth2Constants.ClientId, clientId },
                { OAuth2Constants.ResponseType, responseType }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                values.Add(OAuth2Constants.Scope, scope);
            }

            if (!string.IsNullOrWhiteSpace(redirectUri))
            {
                values.Add(OAuth2Constants.RedirectUri, redirectUri);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                values.Add(OAuth2Constants.State, state);
            }

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                values.Add(OAuth2Constants.Nonce, nonce);
            }

            if (!string.IsNullOrWhiteSpace(loginHint))
            {
                values.Add(OAuth2Constants.LoginHint, loginHint);
            }

            if (!string.IsNullOrWhiteSpace(acrValues))
            {
                values.Add(OAuth2Constants.AcrValues, acrValues);
            }

            if (!string.IsNullOrWhiteSpace(responseMode))
            {
                values.Add(OAuth2Constants.ResponseMode, responseMode);
            }

            return request.Create(Merge(values, ObjectToDictionary(extra)));
        }

        private static Dictionary<string, string> ObjectToDictionary(object values)
        {
            if (values == null)
            {
                return null;
            }

            var dictionary = values as Dictionary<string, string>;
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<string, string>();

            foreach (var prop in values.GetType().GetRuntimeProperties())
            {
                var value = prop.GetValue(values) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    dictionary.Add(prop.Name, value);
                }
            }

            return dictionary;
        }

        private static Dictionary<string, string> Merge(Dictionary<string, string> explicitValues, Dictionary<string, string> additionalValues = null)
        {
            var merged = explicitValues;

            if (additionalValues != null)
            {
                merged =
                    explicitValues.Concat(additionalValues.Where(add => !explicitValues.ContainsKey(add.Key)))
                                         .ToDictionary(final => final.Key, final => final.Value);
            }

            return merged;
        }
    }
}