// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

var oldModule = new Module
{
    Id = Guid.NewGuid(),
    Name = "Neptune",
    Description = "Modulo de operaciones de plataforma",
    IsEnabled = true,
    Pages = [
        new Page
        {
            Id = 1,
            Name = "Company",
            IsEnabled = false,
            Actions = [
                new Action { Id = 11, Name = "Listar empresas" },
                new Action { Id = 12, Name = "Crear empresa" },
                new Action { Id = 13, Name = "Ver detalle de empresa" },
            ]
        },
        //new Page
        //{
        //    //Id = 2,
        //    Name = "Users",
        //    IsEnabled = false,
        //    Actions = [
        //        new Action { Id = 14, Name = "Listar usuarios" },
        //        new Action { Id = 15, Name = "Crear usuario" },
        //        new Action { Id = 16, Name = "Ver detalle" },
        //    ]
        //}
    ]
};

var newModule = new Module
{
    Id = oldModule.Id,
    Name = "Neptune",
    Description = "Modulo de operaciones de plataforma",
    IsEnabled = true,
    Pages = [
        new Page
        {
            Id = 1,
            Name = "Company",
            IsEnabled = true,
            Actions = [
                new Action { Id = 11, Name = "Listar empresas" },
                new Action { Id = 12, Name = "Crear empresa" },
                //new Action { Id = 13, Name = "Ver detalle de compañia" },
            ]
        },
    //    new Page
    //    {
    //        Id = 2,
    //        Name = "Users",
    //        IsEnabled = false,
    //        Actions = [
    //            new Action { Id = 18, Name = "Actualizar usuario" },
    //            //new Action { Id = 15, Name = "Crear usuario" },
    //            //new Action { Id = 16, Name = "Ver detalle" },
    //        ]
    //    }
    ]
};

var calculator = new DifferenceExtractor();

var difference = calculator.GetDelta(oldModule, newModule);

Console.WriteLine(difference);

Console.ReadLine();

public class Module
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsEnabled { get; set; }
    public List<Page> Pages { get; set;}
}

public class Page
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public List<Action> Actions { get; set; }
}

public class Action
{
    public int Id { get; set; }
    public string Name { get; set; }
}