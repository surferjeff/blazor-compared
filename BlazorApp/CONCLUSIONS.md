## praise

Lots of type safety.

Razor templates let me *flexibly* render the model.

React to HTML events with C# code.

## complaints

### Websockets are fragile!

If I take down the server, then an error message immediately appears in the browser, even though I have clicked or interacted with nothing.

If I then restart the server, the web page asks me to reload the page.

In other words, if the websocket connection is interrupted for any reason,
the webpage is broken and the user must reload the page.e

This makes the usual tasks of maintaining a web farm very difficult:
1. Shutting down servers when load drops.
2. Restarting servers with a new version of the app.
3. A web hosting service that supports long-lived websockets.

### Too much magic.

A list of HTML magic that I don't understand:
```
PageTitle
NavLink
Match="NavLinkMatch.All"
```

I don't understand the entirety of `App.razor`:
```html
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```