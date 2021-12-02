using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static System.Console;


public class InstitutoContext : DbContext
{
    public DbSet<Alumno> Alumnos {get; set;}
    public DbSet<Modulo> Modulos{get;set;}  
    public DbSet<Matricula> Matriculas{get; set;}

    public string connString { get; private set; }

    public InstitutoContext()
    {
        var database = "EF12Unai"; // "EF{XX}Nombre" => EF00Santi
        connString = $"Server=185.60.40.210\\SQLEXPRESS,58015;Database={database};User Id=sa;Password=Pa88word;MultipleActiveResultSets=true";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(connString);
    
    protected override void OnModelCreating(ModelBuilder modelBuilder){
         modelBuilder.Entity<Matricula>().HasIndex(m => new
            {
                m.AlumnoId,
                m.ModuloId
            }).IsUnique();
    }

}
public class Alumno
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int AlumnoId { get; set; }
    public string Nombre{ get; set; }
    public int Edad{ get; set; }
    public decimal Efectivo{ get; set; }
    public string Pelo{ get; set;}
}
public class Modulo
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ModuloId { get; set; }
    public string Titulo{get;set;}
    public int Créditos{get; set;}
    public int Curso{get; set;}
}
public class Matricula
{
    public int MatriculaId{get; set;}

    public int AlumnoId {get; set;}

    public int ModuloId{get; set;}

    public Alumno Alumno {get; set;}
    public Modulo Modulo{get; set;}
}

class Program
{
    static void GenerarDatos()
    {
        using (var db = new InstitutoContext())
        {
            // Borrar todo
            db.Alumnos.RemoveRange(db.Alumnos);
            db.Modulos.RemoveRange(db.Modulos);
            db.Matriculas.RemoveRange(db.Matriculas);

            // Añadir Alumnos
            // Id de 1 a 7
            db.Alumnos.Add(new Alumno{AlumnoId=1,Nombre="Unai",Edad=21,Efectivo=7.2m, Pelo="castaño"});
            db.Alumnos.Add(new Alumno{AlumnoId=2,Nombre="Pepe",Edad=20,Efectivo=10.2m, Pelo="rubio"});
            db.Alumnos.Add(new Alumno{AlumnoId=3,Nombre="Marta",Edad=22,Efectivo=14.2m, Pelo="moreno"});
            db.Alumnos.Add(new Alumno{AlumnoId=4,Nombre="Jose",Edad=20,Efectivo=8.2m, Pelo="moreno"});
            db.Alumnos.Add(new Alumno{AlumnoId=5,Nombre="Maria",Edad=21,Efectivo=15.6m, Pelo="castaño"});
            db.Alumnos.Add(new Alumno{AlumnoId=6,Nombre="Paco",Edad=18,Efectivo=14.7m, Pelo="moreno"});
            db.Alumnos.Add(new Alumno{AlumnoId=7,Nombre="Ana",Edad=22,Efectivo=12.1m, Pelo="rubio"});
            // Añadir Módulos
            // Id de 1 a 10
            db.Modulos.Add(new Modulo{ModuloId=1,Titulo="Lengua",Créditos=3,Curso=1});
            db.Modulos.Add(new Modulo{ModuloId=2,Titulo="Matematicas",Créditos=5,Curso=2});
            db.Modulos.Add(new Modulo{ModuloId=3,Titulo="Ingles",Créditos=4,Curso=1});
            db.Modulos.Add(new Modulo{ModuloId=4,Titulo="Frances",Créditos=2,Curso=1});
            db.Modulos.Add(new Modulo{ModuloId=5,Titulo="Educacion Fisica",Créditos=2,Curso=2});
            db.Modulos.Add(new Modulo{ModuloId=6,Titulo="Fisica",Créditos=3,Curso=3});
            db.Modulos.Add(new Modulo{ModuloId=7,Titulo="Quimica",Créditos=3,Curso=2});
            db.Modulos.Add(new Modulo{ModuloId=8,Titulo="Euskera",Créditos=4,Curso=1});
            db.Modulos.Add(new Modulo{ModuloId=9,Titulo="Plastica",Créditos=2,Curso=2});
            db.Modulos.Add(new Modulo{ModuloId=10,Titulo="Teatro",Créditos=1,Curso=1});


            // Matricular Alumnos en Módulos
            foreach (Alumno alumno in db.Alumnos.ToList())
            {
                foreach (Modulo modulo in db.Modulos)
                {
                    db.Add(new Matricula { Alumno = alumno, Modulo = modulo });
                }
            

            db.SaveChanges();
            }
        }
    }

    static void BorrarMatriculaciones()
    {
        using (var db = new InstitutoContext())
        {
        // Borrar las matriculas d
        // AlumnoId multiplo de 3 y ModuloId Multiplo de 2;
       var borrar1= db.Matriculas.Where(x => x.AlumnoId % 3 == 0 && x.ModuloId % 2 == 0);
       foreach(Matricula m in borrar1){
           db.Matriculas.Remove(m);
       }
        // AlumnoId multiplo de 2 y ModuloId Multiplo de 5;
        var borrar2 = db.Matriculas.Where(x => x.AlumnoId % 2 == 0 && x.ModuloId % 5 == 0);
        foreach(Matricula m in borrar2){
           db.Matriculas.Remove(m);
       }
        db.SaveChanges();
        }
    }
    static void RealizarQuery()
    {
        using (var db = new InstitutoContext())
        {
            // Las queries que se piden en el examen
            //Anonymous Type
           var AnonimusT = db.Alumnos.Select(x => 
           new {
            pelo = x.Pelo,
            dinero = x.Efectivo
           }
          );
           //Ordering
           var Ordenado = db.Alumnos.OrderBy(x=> x.Edad);

           //Joining
           var Join = db.Alumnos.Join(db.Matriculas, x=> x.AlumnoId, y=> y.AlumnoId,
           (x,y) => new{
               x.Nombre,
               y.ModuloId
           } );

           //Grouping
           var Group = db.Modulos.GroupBy(
               x => x.ModuloId
           );

           //Paging
           var take = db.Matriculas.Where(
               x =>  x.AlumnoId == 2
           ).Skip(2).Take(3);

           //Single
           var sin = db.Matriculas.Single(
               x => x.AlumnoId == 4
           );

           //Last
           var las = db.Matriculas.Last(
               x => x.ModuloId== 6
               );
           

           //First
           var fir = db.Matriculas.First(
               x => x.AlumnoId == 9
           );

           //ElementAt
            var at = db.Matriculas.ElementAt(
                3
            );

            //Default
            var df = db.Matriculas.Where(
                x => x.AlumnoId == 3
            ).SingleOrDefault();

            //ToArray
            string[] nombre = db.Alumnos.Select(
                x => x.Nombre).ToArray(); 
            
            //ToDictionary
            Dictionary<int, Alumno> id = db.Alumnos.ToDictionary(
                x => x.AlumnoId);
            
            //ToList
            List<Alumno> ordenar = db.Alumnos.Where(
                x => x.AlumnoId < 5).ToList();

            //ToLookup
            ILookup<int, string> Lookup = db.Alumnos.ToLookup(
                x => x.AlumnoId, x => x.Nombre
            ); 
                
            
            //Pruebas de que funcionan
            foreach(var m in Ordenado){
               Console.WriteLine(m.AlumnoId);
           }


        }
    }

    static void Main(string[] args)
    {
        GenerarDatos();
        BorrarMatriculaciones();
        RealizarQuery();
    }

}