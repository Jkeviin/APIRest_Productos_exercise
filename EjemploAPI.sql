use master;
go

drop database EjemploAPI;
go

Create database EjemploAPI;
go

use EjemploAPI
go

create table PRODUCTO(
IdProducto int primary key identity,
CodigoBarra varchar(50) unique,
Nombre varchar(50),
Marca varchar(50),
Categoria varchar(100),
Precio decimal(10,2)
);
go

INSERT INTO PRODUCTO(CodigoBarra,Nombre,Marca,Categoria,Precio) values
('50910010','Monitor Aoc - Curvo Gaming ','AOC','Tecnologia','1200'),
('50910011','IdeaPad 3i','LENOVO','Tecnologia','1700'),
('50910012','SoyMomo Tablet Lite','SOYMOMO','Tecnologia','300'),
('50910013','Lavadora 21 KG WLA-21','WINIA','ElectroHogar','1749'),
('50910014','Congelador 145 Lt Blanco','ELECTROLUX','ElectroHogar','779'),
('50910015','Cafetera TH-130','THOMAS','ElectroHogar','119'),
('50910016','Reloj análogo Hombre 058','GUESS','Accesorios','699'),
('50910017','Billetera de Cuero Mujer Sophie','REYES','Accesorios','270'),
('50910018','Bufanda Rec Mango Mujer','MANGO','Accesorios','169.90'),
('50910019','Sofá Continental 3 Cuerpos','MICA','Muebles','1299'),
('50910020','Futón New Elina 3 Cuerpos','MICA','Muebles','1349'),
('50910021','Mesa Comedor Volterra 6 Sillas','TUHOME','Muebles','624.12');
go

select * from PRODUCTO

go

create procedure sp_lista_productos
as
begin
transaction tx 
select
IdProducto,CodigoBarra,Nombre,
Marca,Categoria,Precio
from PRODUCTO
if  @@ERROR > 0
begin
rollback transaction tx
select 'hubo error' as respuesta
end
else
begin
commit transaction tx
select 'listado de productos realizado con exito' as respuesta
end
go

-- execute sp_lista_productos;
-- go


create procedure sp_guardar_producto(
@codigoBarra varchar(50),
@nombre varchar(50),
@marca varchar(50),
@categoria varchar(100),
@precio decimal(10,2)
)as
begin
transaction tx
insert into 
PRODUCTO(CodigoBarra,Nombre,Marca,Categoria,Precio)
values(@codigoBarra,@nombre,@marca,@categoria,@precio)
if @@ERROR > 0
begin 
rollback transaction tx
select 'hubo error' as respuesta
end
else
begin
commit transaction tx
select 'producto guardado exitosamente' as respuesta
end
go

-- execute sp_guardar_producto '50910067','Mesa Comedor Volterra 6 Sillas','TUHOME','Muebles','624.12';
-- go

select * from PRODUCTO;
go

create procedure sp_editar_producto(
@idProducto int,
@codigoBarra varchar(50) null,
@nombre varchar(50) null,
@marca varchar(50) null,
@categoria varchar(100) null,
@precio decimal(10,2) null
)as
begin
transaction tx
update PRODUCTO set
CodigoBarra = isnull(@codigoBarra,CodigoBarra),
Nombre = isnull(@nombre,Nombre),
Marca = isnull(@marca,Marca),
Categoria = isnull(@categoria,Categoria),
Precio = ISNULL(@precio,Precio)
where IdProducto = @idProducto

if @@error > 0

begin
rollback transaction tx
select 'hubo error' as respuesta
end
else
begin
commit transaction tx
select 'edicion exitosa' as respuesta
end
go


-- execute sp_editar_producto '13','50910067','Mesa Comedor Volterra 8 Sillas','HOMECENTER','Muebles','624.12'
-- go


select * from PRODUCTO;
go

create procedure sp_eliminar_producto(
@idProducto int
)as
begin
transaction tx
delete from PRODUCTO where IdProducto = @idProducto

if @@error > 0
begin
rollback transaction tx
select 'hubo error' as respuesta
end
else
begin
commit transaction tx
select 'eliminacion exitosa' as respuesta
end
go


-- execute  sp_eliminar_producto '13'
-- go
select * from PRODUCTO
go


-------------- USUARIO

create table USUARIO (
	Id int primary key identity,
	Correo varchar(50) unique,
	Clave varchar(50)
	);
go

insert into USUARIO (Correo, Clave) values
	('kevin@gmail.com', 'kevin123');
go

create procedure sp_lista_usuarios
as
begin
	transaction tx 
		select
		id, Correo, Clave
	from USUARIO
	if  @@ERROR > 0
	begin
	rollback transaction tx
	select 'hubo error' as respuesta
	end
	else
	begin
	commit transaction tx
	select 'listado de Usuario realizado con exito' as respuesta
end
go

execute sp_lista_usuarios
go

create procedure sp_guardar_usuario(
@Correo varchar(50),
@Clave varchar(50)
)as
	begin
	transaction tx
		insert into 
		USUARIO (Correo, Clave)
		values(@Correo,@Clave)
	if @@ERROR > 0
	begin 
	rollback transaction tx
	select 'hubo error' as respuesta
	end
	else
	begin
	commit transaction tx
	select 'Usuario guardado exitosamente' as respuesta
	end
go

execute sp_guardar_usuario 'stiven@gmail.com', 'stiven123';
go

create procedure sp_editar_usuario(
@Id int,
@Correo varchar(50),
@Clave varchar(50)
)as
begin
transaction tx
update USUARIO set
Correo = isnull(@Correo,Correo),
Clave = isnull(@Clave,Clave)
where Id = @Id

if @@error > 0

begin
rollback transaction tx
select 'hubo error' as respuesta
end
else
begin
commit transaction tx
select 'edicion exitosa' as respuesta
end
go

execute sp_editar_usuario '1','kevin2@gmai.com', 'kevin2123'
go 

create procedure sp_eliminar_usuario(
@id int
)as
begin
transaction tx
delete from USUARIO where Id = @id

if @@error > 0
begin
rollback transaction tx
select 'hubo error' as respuesta
end
else
begin
commit transaction tx
select 'eliminacion exitosa' as respuesta
end
go

execute sp_eliminar_usuario '1'
go

alter procedure sp_autenticar_usuario (
@Correo varchar(50),
@Clave varchar(50)
)as
    begin
        begin transaction tx
        if exists(select Id, Correo, Clave from USUARIO where correo = @correo and Clave = @Clave)
        begin
            select 'Se encontró un Usuario' as respuesta
            commit transaction tx
        end
        else
        begin
            select 'No se encontró ningun usuario' as respuesta
            rollback transaction tx
        end
    end
go

execute sp_autenticar_usuario 'kevi0n@gmail.com', 'kevin123'