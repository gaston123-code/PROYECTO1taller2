--TRANSACCION: Registrar un nuevo coche + su modelo/marca
BEGIN TRANSACTION;
BEGIN TRY
	INSERT INTO coche (id_coche, precio, nombre, anio_fabricacion, descripcion, imagen, id_modelo, id_estado_coche)
		VALUES (4, 880.00, 'Toyota Corolla Hybrid', 2024, 'Sedán híbrido, eficiente y moderno', 'corolla_hybrid.jpg', 1, 1);
	COMMIT TRANSACTION
END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		PRINT 'Error al agregar coches'
	END CATCH

--TRANSACCION: CREAR UNA RESERVA MAS EL PAGO INICIAL
BEGIN TRANSACTION;
BEGIN TRY
	INSERT INTO reserva (id_reserva, fecha_devolucion, hora_devolucion, precio_total, id_usuario, id_estado_reserva, id_coche)
    VALUES (4, DATEADD(DAY, 7, CAST(GETDATE() AS DATE)), CAST('15:00' AS TIME), 6160.00, 2, 2, 4);

	INSERT INTO detalle_metodo_pago (id_detalle_metodo_pago, id_reserva, id_metodo_pago, importe) VALUES (4, 4, 2, 6160.00);

	UPDATE coche
	SET id_estado_coche = 2
	WHERE id_coche = 4

	COMMIT transaction;
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION;
	PRINT 'Error al realizar reserva'
END CATCH

--TRANSACSION: FINALIZAR UNA RESERVA
BEGIN TRANSACTION;
BEGIN TRY
	UPDATE reserva
	SET id_estado_reserva = 3
	WHERE id_reserva = 1

	UPDATE coche
	SET id_estado_coche = 1
	WHERE id_coche = (
		SELECT id_coche
		FROM reserva
		WHERE id_reserva = 1
		)
	COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION;
	PRINT 'error al finalizar reserva';
END CATCH
