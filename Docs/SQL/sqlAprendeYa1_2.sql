CREATE DATABASE AprendeYa;
GO

USE AprendeYa;
GO

-- =========================
-- PERSONA / USUARIO / ROL
-- =========================

CREATE TABLE rol (
    id_rol INT PRIMARY KEY IDENTITY,
    nombre VARCHAR(50) NOT NULL
);

CREATE TABLE persona (
    id_persona INT PRIMARY KEY IDENTITY,
    nombres VARCHAR(100),
    apellidos VARCHAR(100),
    correo VARCHAR(100),
    telefono VARCHAR(20)
);

-- =========================
-- TABLA USUARIO
-- =========================
CREATE TABLE usuario (
    id_usuario          INT IDENTITY(1,1) PRIMARY KEY, -- PK autoincremental
    id_persona          INT UNIQUE,                    -- Relación con persona
    id_rol              INT,                           -- Relación con rol
    username            VARCHAR(50) NOT NULL UNIQUE,   -- Nombre de usuario único
    contrasena_hash     VARCHAR(255) NOT NULL,         -- Contraseña en formato hash
    contrasena_literal  VARCHAR(255) NULL,             -- Contraseña en texto plano (solo pruebas)
    estado              BIT DEFAULT 1,                 -- Estado activo/inactivo
    fecha_registro      DATETIME NOT NULL DEFAULT GETDATE(),
    fecha_actualizacion DATETIME NULL,
    FOREIGN KEY (id_persona) REFERENCES persona(id_persona),
    FOREIGN KEY (id_rol) REFERENCES rol(id_rol)
);

-- =========================
-- PROCEDIMIENTO: Insertar Usuario con hash
-- =========================
-- Este procedimiento recibe la contraseña en texto plano,
-- la convierte en hash y guarda ambos valores en la tabla.
-- NOTA: En producción, lo ideal es NO guardar contrasena_literal.
-- Se incluye aquí solo para pruebas y auditoría.
-- =========================

CREATE PROCEDURE sp_insertar_usuario
    @id_persona INT,
    @id_rol INT,
    @username VARCHAR(50),
    @contrasena_literal VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Generar hash de la contraseña usando HASHBYTES (ejemplo con SHA2_256)
    DECLARE @contrasena_hash VARCHAR(255);
    SET @contrasena_hash = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @contrasena_literal), 2);

    -- Insertar el usuario con ambos valores
    INSERT INTO usuario (id_persona, id_rol, username, contrasena_hash, contrasena_literal, estado, fecha_registro)
    VALUES (@id_persona, @id_rol, @username, @contrasena_hash, @contrasena_literal, 1, GETDATE());
END;
GO

-- =========================
-- PROCEDIMIENTO: Actualizar Contraseña
-- =========================
-- Permite cambiar la contraseña de un usuario.
-- Convierte la nueva contraseña literal en hash automáticamente.
-- =========================

CREATE PROCEDURE sp_actualizar_contrasena
    @id_usuario INT,
    @nueva_contrasena_literal VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @nueva_contrasena_hash VARCHAR(255);
    SET @nueva_contrasena_hash = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @nueva_contrasena_literal), 2);

    UPDATE usuario
    SET contrasena_hash = @nueva_contrasena_hash,
        contrasena_literal = @nueva_contrasena_literal,
        fecha_actualizacion = GETDATE()
    WHERE id_usuario = @id_usuario;
END;
GO



-- =========================
-- HERENCIA
-- =========================

CREATE TABLE adminstrador (
    id_admin INT PRIMARY KEY IDENTITY,
    id_persona INT UNIQUE,
    nivel_acceso VARCHAR(50),
    FOREIGN KEY (id_persona) REFERENCES persona(id_persona)
);

CREATE TABLE alumno (
    id_alumno INT PRIMARY KEY IDENTITY,
    id_persona INT UNIQUE,
    fecha_registro DATE,
    estado_academico VARCHAR(50),
    FOREIGN KEY (id_persona) REFERENCES persona(id_persona)
);

CREATE TABLE instructor (
    id_instructor INT PRIMARY KEY IDENTITY,
    id_persona INT UNIQUE,
    especialidad VARCHAR(100),
    descripcion VARCHAR(255),
    FOREIGN KEY (id_persona) REFERENCES persona(id_persona)
);

-- =========================
-- DIRECCION
-- =========================

CREATE TABLE direccion (
    id_direccion INT PRIMARY KEY IDENTITY,
    id_persona INT,
    pais VARCHAR(50),
    ciudad VARCHAR(50),
    distrito VARCHAR(50),
    direccion VARCHAR(150),
    referencia VARCHAR(150),
    principal BIT,
    FOREIGN KEY (id_persona) REFERENCES persona(id_persona)
);

-- =========================
-- CURSOS
-- =========================

CREATE TABLE nivel (
    id_nivel INT PRIMARY KEY IDENTITY,
    nombre VARCHAR(50)
);

CREATE TABLE curso (
    id_curso INT PRIMARY KEY IDENTITY,
    titulo VARCHAR(150),
    descripcion TEXT,
    precio DECIMAL(10,2),
    id_nivel INT,
    id_instructor INT,
    estado VARCHAR(50),
    FOREIGN KEY (id_nivel) REFERENCES nivel(id_nivel),
    FOREIGN KEY (id_instructor) REFERENCES instructor(id_instructor)
);

CREATE TABLE modulo (
    id_modulo INT PRIMARY KEY IDENTITY,
    id_curso INT,
    titulo VARCHAR(150),
    descripcion TEXT,
    orden INT,
    FOREIGN KEY (id_curso) REFERENCES curso(id_curso)
);

CREATE TABLE tema (
    id_tema INT PRIMARY KEY IDENTITY,
    id_modulo INT,
    titulo VARCHAR(150),
    descripcion TEXT,
    orden INT,
    FOREIGN KEY (id_modulo) REFERENCES modulo(id_modulo)
);

CREATE TABLE contenido (
    id_contenido INT PRIMARY KEY IDENTITY,
    id_tema INT,
    tipo VARCHAR(50),
    url VARCHAR(255),
    texto TEXT,
    duracion INT,
    orden INT,
    FOREIGN KEY (id_tema) REFERENCES tema(id_tema)
);

-- =========================
-- EJERCICIOS
-- =========================

CREATE TABLE ejercicio (
    id_ejercicio INT PRIMARY KEY IDENTITY,
    id_tema INT,
    pregunta TEXT,
    tipo VARCHAR(50),
    FOREIGN KEY (id_tema) REFERENCES tema(id_tema)
);

CREATE TABLE alternativa (
    id_alternativa INT PRIMARY KEY IDENTITY,
    id_ejercicio INT,
    texto VARCHAR(255),
    es_correcta BIT,
    FOREIGN KEY (id_ejercicio) REFERENCES ejercicio(id_ejercicio)
);

CREATE TABLE respuesta_usuario (
    id_respuesta INT PRIMARY KEY IDENTITY,
    id_ejercicio INT,
    id_usuario INT,
    respuesta TEXT,
    es_correcta BIT,
    fecha DATETIME,
    FOREIGN KEY (id_ejercicio) REFERENCES ejercicio(id_ejercicio),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);

-- =========================
-- PROGRESO
-- =========================

CREATE TABLE progreso_tema (
    id_progreso INT PRIMARY KEY IDENTITY,
    id_usuario INT,
    id_tema INT,
    completado BIT,
    fecha DATETIME,
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
    FOREIGN KEY (id_tema) REFERENCES tema(id_tema)
);

-- =========================
-- CARRITO
-- =========================

CREATE TABLE carrito (
    id_carrito INT PRIMARY KEY IDENTITY,
    id_usuario INT,
    estado VARCHAR(50),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);

CREATE TABLE carrito_detalle (
    id_detalle INT PRIMARY KEY IDENTITY,
    id_carrito INT,
    id_curso INT,
    precio DECIMAL(10,2),
    FOREIGN KEY (id_carrito) REFERENCES carrito(id_carrito),
    FOREIGN KEY (id_curso) REFERENCES curso(id_curso)
);

CREATE TABLE cartera (
    id_cartera INT PRIMARY KEY IDENTITY,
    id_carrito INT UNIQUE,
    subtotal DECIMAL(10,2),
    descuento DECIMAL(10,2),
    total DECIMAL(10,2),
    FOREIGN KEY (id_carrito) REFERENCES carrito(id_carrito)
);

-- =========================
-- VENTAS
-- =========================

CREATE TABLE venta (
    id_venta INT PRIMARY KEY IDENTITY,
    id_usuario INT,
    fecha DATETIME,
    total DECIMAL(10,2),
    estado VARCHAR(50),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);

CREATE TABLE venta_curso (
    id_venta_curso INT PRIMARY KEY IDENTITY,
    id_venta INT,
    id_curso INT,
    precio DECIMAL(10,2),
    FOREIGN KEY (id_venta) REFERENCES venta(id_venta),
    FOREIGN KEY (id_curso) REFERENCES curso(id_curso)
);

-- =========================
-- FORO
-- =========================

CREATE TABLE foro (
    id_foro INT PRIMARY KEY IDENTITY,
    id_curso INT,
    FOREIGN KEY (id_curso) REFERENCES curso(id_curso)
);

CREATE TABLE tema_foro (
    id_tema INT PRIMARY KEY IDENTITY,
    id_foro INT,
    id_usuario INT,
    titulo VARCHAR(150),
    contenido TEXT,
    fecha DATETIME,
    FOREIGN KEY (id_foro) REFERENCES foro(id_foro),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);

CREATE TABLE respuesta_foro (
    id_respuesta INT PRIMARY KEY IDENTITY,
    id_tema INT,
    id_usuario INT,
    contenido TEXT,
    fecha DATETIME,
    FOREIGN KEY (id_tema) REFERENCES tema_foro(id_tema),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);

-- =========================
-- CERTIFICADO
-- =========================

CREATE TABLE certificado (
    id_certificado INT PRIMARY KEY IDENTITY,
    id_usuario INT,
    id_curso INT,
    nombre_estudiante VARCHAR(150),
    nombre_instructor VARCHAR(150),
    titulo_curso VARCHAR(150),
    fecha_emision DATETIME,
    codigo VARCHAR(100) UNIQUE,
    estado VARCHAR(50),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
    FOREIGN KEY (id_curso) REFERENCES curso(id_curso)
);