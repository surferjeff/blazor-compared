use axum::extract::OriginalUri;
use axum::http::StatusCode;
use axum::response::{Html, IntoResponse};
use axum::routing::get;
use axum::Router;
use askama::Template;
use tower_http::services::ServeDir;

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
    pub fn survey_html<'a> (&'a self, title: &'a str) -> SurveyTemplate<'a> {
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


async fn index(uri: OriginalUri) -> impl IntoResponse {
    render_main_layout_page("Home", uri.0.path(), &HomeTemplate {})
}

async fn about(uri: OriginalUri) -> impl IntoResponse {
    render_main_layout_page("About", uri.0.path(), &AboutTemplate {})
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
        .fallback_service(ServeDir::new("static/"));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3456").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}