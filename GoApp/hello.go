package main

import (
	"context"
	"errors"
	"fmt"
	"html/template"
	"io"
	"log"
	"strings"
	"time"

	"github.com/a-h/templ"
	"github.com/gofiber/fiber/v2"
)

type MyViews struct {
	templates map[string]*template.Template
}

func reverse(numbers []string) []string {
	for i := 0; i < len(numbers)/2; i++ {
		j := len(numbers) - i - 1
		numbers[i], numbers[j] = numbers[j], numbers[i]
	}
	return numbers
}

func (v *MyViews) Load() error {
	v.templates = make(map[string]*template.Template)

	parsePage := func(templates ...string) error {
		name := templates[0]
		templates = append(templates, "MainLayout", "NavMenu", "_Layout")
		templates = reverse(templates)
		for i := 0; i < len(templates); i++ {
			templates[i] = fmt.Sprintf("templates/%s.html", templates[i])
		}
		tmpl, error := template.ParseFiles(templates...)
		if tmpl != nil {
			v.templates[name] = tmpl
			return nil
		} else {
			return error
		}
	}

	error := parsePage("Index", "SurveyPrompt")
	if error != nil {
		return error
	}

	error = parsePage("About")
	if error != nil {
		return error
	}

	error = parsePage("Counter")
	if error != nil {
		return error
	}

	error = parsePage("FetchData")
	if error != nil {
		return error
	}

	tmpl, error := template.ParseFiles("templates/Forecasts.html")
	if error != nil {
		return error
	} else {
		v.templates["Forecasts"] = tmpl
	}

	return nil
}

func wrapWithLayout(c *fiber.Ctx, title string,
	component templ.Component) templ.Component {
	main := main_layout(nav_menu(c.Route().Path), component)
	headers := c.GetReqHeaders()
	if headers["Hx-Boosted"] == "true" {
		return boosted_layout(title, main)
	} else {
		return layout(title, main)
	}
}

func (v *MyViews) Render(w io.Writer, templateName string,
	data interface{}, _ignored ...string) error {
	if templateName == "Index" {
		c := data.(*fiber.Ctx)
		layout := wrapWithLayout(c, "Home", index())
		return layout.Render(context.Background(), w)
	}
	if templateName == "Counter" {
		c := data.(*fiber.Ctx)
		layout := wrapWithLayout(c, "Counter", counter(0))
		return layout.Render(context.Background(), w)
	}
	tmpls := strings.Split(templateName, " ")
	if len(tmpls) == 1 {
		tmpl := v.templates[templateName]
		if tmpl == nil {
			return errors.New(fmt.Sprintf("No template named %s", templateName))
		}
		return tmpl.Execute(w, data)
	} else if len(tmpls) == 2 {
		tmpl := v.templates[tmpls[0]]
		if tmpl == nil {
			return errors.New(fmt.Sprintf("No template named %s", tmpls[0]))
		}
		return tmpl.ExecuteTemplate(w, tmpls[1], data)
	} else {
		return errors.New(fmt.Sprintf("Bad template name '%s'", templateName))
	}
}

func dataFromContext(c *fiber.Ctx) fiber.Map {
	cmap := fiber.Map{}
	headers := c.GetReqHeaders()
	if headers["Hx-Boosted"] == "true" {
		cmap["HxBoosted"] = true
	}
	cmap["Path"] = c.Route().Path
	return cmap
}

type Handler func(*fiber.Ctx) (templ.Component, error)

type TemplViews struct {
	templates map[string]Handler
	fallback  fiber.Views
}

func (v *TemplViews) Load() error {
	if v.fallback != nil {
		return v.fallback.Load()
	}
	return nil
}

func (v *TemplViews) Render(w io.Writer, templateName string,
	data interface{}, args ...string) error {
	f := v.templates[templateName]
	if f != nil {
		c := data.(*fiber.Ctx)
		component, err := f(c)
		if err != nil {
			err = component.Render(context.Background(), w)
		}
		return err
	} else {
		return v.fallback.Render(w, templateName, data, args...)
	}
}

type AppBuilder struct {
	views *TemplViews
	app   *fiber.App
}

func (b AppBuilder) Get(path string, handler Handler) fiber.Router {
	b.views.templates[path] = handler
	return b.app.Get(path, func(c *fiber.Ctx) error {
		return c.Render(path, c)
	})
}

func main() {
	templViews := new(TemplViews)
	templViews.templates = make(map[string]Handler)
	templViews.fallback = new(MyViews)
	app := fiber.New(fiber.Config{
		Views: templViews,
	})
	builder := new(AppBuilder)
	builder.app = app
	builder.views = templViews

	app.Static("/", "./wwwroot")

	app.Get("/hello", func(c *fiber.Ctx) error {
		c.Set("Vary", "HX-Boosted")
		return c.Render("Hello", dataFromContext(c))
	})
	builder.Get("/", func(c *fiber.Ctx) (templ.Component, error) {
		c.Set("Vary", "HX-Boosted")
		return wrapWithLayout(c, "Home", index()), nil
	})
	app.Get("/about", func(c *fiber.Ctx) error {
		c.Set("Vary", "HX-Boosted")
		return c.Render("About", dataFromContext(c))
	})
	app.Get("/counter", func(c *fiber.Ctx) error {
		c.Set("Vary", "HX-Boosted")
		return c.Render("Counter", c)
	})
	app.Get("/increment", func(c *fiber.Ctx) error {
		count := c.QueryInt("count", 0)
		return c.Render("Counter main-article", fiber.Map{
			"CurrentCount": count,
			"NextCount":    count + 1,
		})
	})
	app.Get("/fetchdata", func(c *fiber.Ctx) error {
		c.Set("Vary", "HX-Boosted")
		return c.Render("FetchData", dataFromContext(c))
	})
	app.Post("/forecasts", func(c *fiber.Ctx) error {
		return c.Render("Forecasts", getForecasts(time.Now()))
	})

	log.Fatal(app.Listen(":3000"))
}
