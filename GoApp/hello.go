package main

import (
	"text/template"

	"github.com/gofiber/fiber/v2"
)

func main() {
	app := fiber.New()

	app.Static("/", "./wwwroot")

	tmpl := template.Must(template.ParseFiles(
		"templates/Index.html",
		"templates/NavMenu.html",
		"templates/MainLayout.html",
		"templates/_Layout.html"))

	app.Get("/", func(c *fiber.Ctx) error {
		return tmpl.Execute(c, "layout")
	})

	app.Listen(":3000")
}
