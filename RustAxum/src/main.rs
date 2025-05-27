use axum::extract::{OriginalUri, Path};
use axum::http::StatusCode;
use axum::response::{Html, IntoResponse};
use axum::routing::get;
use axum::Router;
use askama::Template;
use tower_http::services::ServeDir;

#[derive(Template)]
#[template(path = "index.html")]
struct HelloTemplate<'a> {
    name: &'a str,
}

#[derive(Template)]
#[template(path = "goodday.html")]
struct GoodDayTemplate { }

#[derive(Template)]
#[template(path = "layout.html")]
struct LayoutTemplate<'a, T: Template> {
    title: &'a str,
    main_html: &'a T
}

#[derive(Template)]
#[template(path = "main_layout.html")]
struct MainLayoutTemplate<'a, NM: Template, MA:Template> {
    nav_menu_html: &'a NM,
    main_article_html: &'a MA
}

#[derive(Template)]
#[template(path = "nav_menu.html")]
struct NavMenuTemplate<'a> {
    path: &'a str,
}

#[derive(Template)]
#[template(path = "about.html")]
struct AboutTemplate { }

#[derive(Template)]
#[template(path = "home.html")]
struct HomeTemplate { }

#[derive(Template)]
#[template(path = "survey.html")]
struct SurveyTemplate<'a> { 
    title: &'a str
}


impl HomeTemplate {
    pub fn survey<'a> (&'a self, title: &'a str) -> SurveyTemplate<'a> {
        SurveyTemplate { title }
    }
}

trait IntoHtml {
    fn into_html(self) -> impl IntoResponse;
}

impl IntoHtml for askama::Result<String> {
    fn into_html(self) -> impl IntoResponse {
        match self {
            Ok(html) => Html(html).into_response(),
            Err(err) => (
                StatusCode::INTERNAL_SERVER_ERROR,
                format!("Failed to render template. Error: {err}"),
            ).into_response(),
        }        
    }
}

async fn hello(Path(name): Path<String>) -> impl IntoResponse {
    let template = HelloTemplate { name: name.as_str() };
    template.render().into_html()
}

async fn goodday() -> impl IntoResponse {
    let goodday = GoodDayTemplate {};
    let template = LayoutTemplate {
        title: "Good Day",
        main_html: &goodday
    };
    template.render().into_html()
}

async fn index() -> impl IntoResponse {
    LayoutTemplate {
        title: "Home",
        main_html: &MainLayoutTemplate {
            nav_menu_html: &NavMenuTemplate { path: "/" },
            main_article_html: &HomeTemplate {},
        }
    }.render().into_html()
}

async fn about(uri: OriginalUri) -> impl IntoResponse {
    render_main_layout_page("Home", uri.0.path(), &AboutTemplate {})
}

fn render_main_layout_page<'a, MA>(
    title: &'a str,
    path: &'a str,
    main_article_html: &'a MA,
) -> impl IntoResponse
where
    MA: Template,
{
    let main_layout = MainLayoutTemplate {
        nav_menu_html: &NavMenuTemplate { path },
        main_article_html,
    };
    LayoutTemplate {
        title,
        main_html: &main_layout,
    }
    .render()
    .into_html()
}



#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/",  get(index))
        .route("/about", get(about))
        .route("/hello/{name}", get(hello))
        .route("/goodday", get(goodday))
        .fallback_service(ServeDir::new("static/"));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3456").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}