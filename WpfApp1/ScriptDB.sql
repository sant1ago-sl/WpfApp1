-- ============================================
-- SCRIPT: Crear Base de Datos ReservasDB
-- Motor: SQL Server LocalDB
-- NOTA: Ejecutar con: sqlcmd -S "(localdb)\MSSQLLocalDB" -i ScriptDB.sql -f 65001
-- ============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ReservasDB')
BEGIN
    CREATE DATABASE ReservasDB;
END
GO

USE ReservasDB;
GO

-- ============ TABLA: Usuarios ============
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Password NVARCHAR(100) NOT NULL,
        NombreCompleto NVARCHAR(100) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM Usuarios)
BEGIN
    INSERT INTO Usuarios (Username, Password, NombreCompleto) VALUES
    (N'jperez',    N'123456', N'Juan Pérez García'),
    (N'mlopez',    N'123456', N'María López Torres'),
    (N'crodriguez',N'123456', N'Carlos Rodríguez Sánchez'),
    (N'lgonzalez', N'123456', N'Laura González Martínez'),
    (N'pmartinez', N'123456', N'Pedro Martínez Ruiz'),
    (N'asanchez',  N'123456', N'Ana Sánchez Díaz'),
    (N'rramirez',  N'123456', N'Rosa Ramírez Flores'),
    (N'dtorres',   N'123456', N'Diego Torres Vargas'),
    (N'fcastro',   N'123456', N'Fabiola Castro Ríos'),
    (N'gflores',   N'123456', N'Gustavo Flores Morales');
END
GO

-- ============ TABLA: Aulas ============
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Aulas')
BEGIN
    CREATE TABLE Aulas (
        AulaId INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL,
        Capacidad INT NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM Aulas)
BEGIN
    INSERT INTO Aulas (Nombre, Capacidad) VALUES
    (N'Aula 101', 30),
    (N'Aula 102', 25),
    (N'Aula 103', 40),
    (N'Aula 201', 30),
    (N'Aula 202', 20),
    (N'Aula 203', 35),
    (N'Aula 301', 50),
    (N'Aula 302', 45),
    (N'Aula 303', 60),
    (N'Laboratorio A', 20),
    (N'Laboratorio B', 25),
    (N'Sala de Conferencias', 80),
    (N'Sala de Reuniones 1', 12),
    (N'Sala de Reuniones 2', 10),
    (N'Auditorio', 120);
END
GO

-- ============ TABLA: Reservas ============
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservas')
BEGIN
    CREATE TABLE Reservas (
        ReservaId INT IDENTITY(1,1) PRIMARY KEY,
        AulaId INT NOT NULL,
        UsuarioId INT NOT NULL,
        Fecha DATE NOT NULL,
        Hora TIME NOT NULL,
        Motivo NVARCHAR(200) NOT NULL,
        CONSTRAINT FK_Reservas_Aulas FOREIGN KEY (AulaId) REFERENCES Aulas(AulaId),
        CONSTRAINT FK_Reservas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM Reservas)
BEGIN
    INSERT INTO Reservas (AulaId, UsuarioId, Fecha, Hora, Motivo) VALUES
    (1,  1, '2026-09-03', '08:00', N'Clase de Matemáticas'),
    (2,  2, '2026-09-03', '10:00', N'Taller de Programación'),
    (3,  3, '2026-09-03', '14:00', N'Seminario de Física'),
    (4,  4, '2026-09-04', '08:00', N'Clase de Química'),
    (5,  5, '2026-09-04', '10:00', N'Reunión de tutores'),
    (6,  6, '2026-09-04', '16:00', N'Clase de Inglés'),
    (7,  7, '2026-09-05', '08:00', N'Conferencia invitada'),
    (8,  8, '2026-09-05', '11:00', N'Taller de Robótica'),
    (9,  9, '2026-09-05', '15:00', N'Clase de Historia'),
    (10, 10, '2026-09-06', '09:00', N'Laboratorio de Física'),
    (11, 1,  '2026-09-06', '13:00', N'Laboratorio de Química'),
    (12, 2,  '2026-09-06', '17:00', N'Charla magistral'),
    (13, 3,  '2026-09-07', '08:00', N'Reunión de proyecto'),
    (14, 4,  '2026-09-07', '10:00', N'Entrevistas laborales'),
    (15, 5,  '2026-09-07', '14:00', N'Auditoría interna'),
    (1,  6,  '2026-09-08', '09:00', N'Clase de Matemáticas'),
    (2,  7,  '2026-09-08', '11:00', N'Taller de Base de Datos'),
    (3,  8,  '2026-09-08', '15:00', N'Seminario de Redes'),
    (4,  9,  '2026-09-09', '08:00', N'Clase de Álgebra'),
    (5,  10, '2026-09-09', '10:00', N'Reunión de docentes'),
    (6,  1,  '2026-09-09', '16:00', N'Clase de Literatura'),
    (7,  2,  '2026-09-10', '08:00', N'Taller de Innovación'),
    (8,  3,  '2026-09-10', '13:00', N'Defensa de tesis'),
    (9,  4,  '2026-09-10', '16:00', N'Clase de Estadística'),
    (10, 5,  '2026-09-11', '09:00', N'Práctica de laboratorio'),
    (11, 6,  '2026-09-11', '14:00', N'Investigación dirigida'),
    (12, 7,  '2026-09-11', '18:00', N'Evento cultural'),
    (13, 8,  '2026-09-12', '08:00', N'Taller de Liderazgo'),
    (14, 9,  '2026-09-12', '10:00', N'Capacitación docente'),
    (15, 10,'2026-09-12', '15:00', N'Cierre de semestre');
END
GO

PRINT 'Base de datos ReservasDB creada exitosamente.';
GO
