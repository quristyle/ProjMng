using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ProjMngWasm.Services;

public class AuthStateProvider : AuthenticationStateProvider {
    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {


        AuthenticationState acs = new AuthenticationState(
 new System.Security.Claims.ClaimsPrincipal(

new ClaimsIdentity(

new[] {

new Claim("USER_ID", "test")
, new Claim("USER_GRP_ID", "")

}
, "JwtAuth"

)

 )

        );
        return acs;


    }
}
