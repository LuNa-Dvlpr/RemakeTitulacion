-- Asegúrate de estar usando la base de datos correcta
USE Tutorias;
GO

-- Insertamos los 10 registros en la tabla Usuarios (TIPO = 1 para Profesor)
INSERT INTO Usuarios ([User], Pass, TIPO, Visibilidad) 
VALUES 
('isabelvega', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('robertosan', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('lauranuñez', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('javiermora', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('elenacastr', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('davidorteg', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('carmenrios', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('pablogomez', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('sofiaflore', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1),
('miguelcruz', '25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68', 1, 1);
GO

-- Insertamos los detalles en la tabla Profesor, vinculándolos a los usuarios recién creados
-- NOTA: Este script asume que los Id_Usuario se generarán en secuencia.
-- Si ya tienes usuarios en la tabla, deberás ajustar los números de Id_Usuario.
INSERT INTO Profesor (Id_Usuario, Nombre, Apellido_Pat, Apellido_Mat, Correo, Grupo, HorasTotales, HorasTutoria)
VALUES 
-- Asumiendo que el siguiente ID disponible es el 4. ¡Ajusta este número si es necesario!
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'isabelvega'), 'Isabel', 'Vega', 'Reyes', 'isabel.vega@email.com', '6IV11', 20, 8),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'robertosan'), 'Roberto', 'Sanz', 'Perez', 'roberto.sanz@email.com', '6IV11', 24, 5),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'lauranuñez'), 'Laura', 'Nuñez', 'Jimenez', 'laura.nuñez@email.com', '6IV11', 22, 10),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'javiermora'), 'Javier', 'Mora', 'Guerrero', 'javier.mora@email.com', '6IV11', 18, 7),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'elenacastr'), 'Elena', 'Castro', 'Ruiz', 'elena.castro@email.com', '6IV11', 20, 9),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'davidorteg'), 'David', 'Ortega', 'Serrano', 'david.ortega@email.com', '6IV11', 25, 6),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'carmenrios'), 'Carmen', 'Rios', 'Navarro', 'carmen.rios@email.com', '6IV11', 21, 10),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'pablogomez'), 'Pablo', 'Gomez', 'Alonso', 'pablo.gomez@email.com', '6IV11', 19, 4),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'sofiaflore'), 'Sofia', 'Flores', 'Diaz', 'sofia.flores@email.com', '6IV11', 23, 8),
((SELECT Id_Usuario FROM Usuarios WHERE [User] = 'miguelcruz'), 'Miguel', 'Cruz', 'Santos', 'miguel.cruz@email.com', '6IV11', 20, 7);
GO