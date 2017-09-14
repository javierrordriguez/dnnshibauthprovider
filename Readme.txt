University of Florida

Steps to install: 

1. Install the module: "ShibInstall.zip" which is located under Downloads.

2. Using the admin account, add 3 new pages: a login page, a logout page, and a role mappings page. 
The login and logout pages can be called anything you want.
These two pages should be viewed by all users. On the login page add the module: Account Login.

Add a role mapping page. This must be named "RoleMappings.aspx" and should only be viewable by admins.
On the role mapping page, add the module: Shibboleth_Authentication. This will be the Radgrid
where administrators can map shibboleth roles to existing dnn roles. 

3. Go into the settings module and enable shibboleth and select your new login and logout pages from the 
dropdown lists.

If at anytime during the initial configuration, you need to get back to the original DNN login screen, 
and you forgot to setup your login page, you can make your shibboleth signon a superuser and then
add the pages as usual. 

4. Modify your web config: 

Add this line to the <httpModules> </httpModules> section of the web.config: 

<add name="Authentication" type="UF.Research.Authentication.Shibboleth.HttpModules.AuthenticationModule, UF.Research.Authentication.Shibboleth"/>

5. Secure your "~/DesktopModules/AuthenticationServices/Shibboleth/Login" directory to Shibboleth in the Shibboleth.xml file. Here's an
example of the code we used when testing on domain dnn1.research.ufl.edu: 

<Host name="dnn1.research.ufl.edu:9999" applicationId="urn:edu:ufl:alpha1:99999" >

<Path name="DesktopModules" authType="shibboleth" requireSession="false">

<Path name="AuthenticationServices" authType="shibboleth" requireSession="false">

<Path name="Shibboleth" authType="shibboleth" requireSession="false">

<Path name="Login" authType="shibboleth" requireSession="true">

</Path>

</Path>

</Path>

</Path>

</Host>



