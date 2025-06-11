using VolvoFleetProgram.Controllers;
using VolvoFleetProgram.Models;
using VolvoFleetProgram.Services;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using VolvoFleetProgram.Enums;
using VolvoFleetProgram.Entities.Request;
using VolvoFleetProgram.Entities.DTOs;

namespace VolvoFleetProgram.Tests.ControllerTests
{
    public class VehiclesControllerTests
    {
        private readonly VehiclesController _controller;
        private readonly Mock<IVehicleService> _vehicleServiceMock;

        public VehiclesControllerTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>();
            _controller = new VehiclesController(_vehicleServiceMock.Object);
        }

        #region GetAllVehicles Tests

        [Fact]
        public void GetAllVehicles_ReturnsOk_WithVehiclesList()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle(vehicleType.Car, new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 }, "Vermelho"),
                new Vehicle(vehicleType.Truck, new ChassisIdentifier { ChassisSeries = "DEF", ChassisNumber = 456 }, "Azul")
            };
            _vehicleServiceMock.Setup(s => s.GetAllVehicles()).Returns(vehicles);

            // Act
            var result = _controller.GetAllVehicles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<VehicleResponseListDTO>(okResult.Value);
            Assert.Equal(2, response.Total);
            Assert.Equal(2, response.Vehicles.Count());
        }

        [Fact]
        public void GetAllVehicles_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var vehicles = new List<Vehicle>();
            _vehicleServiceMock.Setup(s => s.GetAllVehicles()).Returns(vehicles);

            // Act
            var result = _controller.GetAllVehicles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<VehicleResponseListDTO>(okResult.Value);
            Assert.Equal(0, response.Total);
            Assert.Empty(response.Vehicles);
        }

        [Fact]
        public void GetAllVehicles_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            _vehicleServiceMock.Setup(s => s.GetAllVehicles()).Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetAllVehicles();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Erro interno do server", statusResult.Value.ToString());
        }

        [Fact]
        public void GetAllVehicles_ReturnsCorrectTotal_WhenVehiclesListHasMultipleItems()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle(vehicleType.Car, new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 }, "Vermelho"),
                new Vehicle(vehicleType.Truck, new ChassisIdentifier { ChassisSeries = "DEF", ChassisNumber = 456 }, "azul"),
                new Vehicle(vehicleType.Bus, new ChassisIdentifier { ChassisSeries = "GHI", ChassisNumber = 789 }, "Verde")
            };
            _vehicleServiceMock.Setup(s => s.GetAllVehicles()).Returns(vehicles);

            // Act
            var result = _controller.GetAllVehicles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<VehicleResponseListDTO>(okResult.Value);
            Assert.Equal(3, response.Total);
            Assert.Equal(vehicles.Count, response.Vehicles.Count());
        }

        #endregion

        #region GetVehicleDetails Tests

        [Fact]
        public void GetVehicleDetails_ReturnsOk_WhenVehicleExists()
        {
            // Arrange
            var chassisId = new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 };
            var vehicle = new Vehicle(vehicleType.Car, chassisId, "Vermelho");
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Returns(vehicle);

            // Act
            var result = _controller.GetVehicleDetails("ABC", 123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicle = Assert.IsAssignableFrom<Vehicle>(okResult.Value);
            Assert.Equal(vehicle.Color, returnedVehicle.Color);
            Assert.Equal(vehicle.ChassisId.ChassisSeries, returnedVehicle.ChassisId.ChassisSeries);
            Assert.Equal(vehicle.ChassisId.ChassisNumber, returnedVehicle.ChassisId.ChassisNumber);
        }

        [Fact]
        public void GetVehicleDetails_ReturnsNotFound_WhenVehicleDoesNotExist()
        {
            // Arrange
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Returns((Vehicle)null);

            // Act
            var result = _controller.GetVehicleDetails("DEF", 456);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetVehicleDetails_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetVehicleDetails("ABC", 123);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Erro interno do server", statusResult.Value.ToString());
        }

        [Fact]
        public void GetVehicleDetails_CreatesCorrectChassisIdentifier_WithProvidedParameters()
        {
            // Arrange
            var expectedSeries = "XYZ";
            var expectedNumber = 999;
            var vehicle = new Vehicle(vehicleType.Car, new ChassisIdentifier { ChassisSeries = expectedSeries, ChassisNumber = expectedNumber }, "Vermelho");

            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.Is<ChassisIdentifier>(
                c => c.ChassisSeries == expectedSeries && c.ChassisNumber == expectedNumber)))
                .Returns(vehicle);

            // Act
            var result = _controller.GetVehicleDetails(expectedSeries, expectedNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicle = Assert.IsAssignableFrom<Vehicle>(okResult.Value);
            Assert.Equal(expectedSeries, returnedVehicle.ChassisId.ChassisSeries);
            Assert.Equal(expectedNumber, returnedVehicle.ChassisId.ChassisNumber);
        }

        #endregion

        #region CreateVehicle Tests

        [Fact]
        public void CreateVehicle_ReturnsCreatedAtAction_WhenVehicleIsCreatedSuccessfully()
        {
            // Arrange
            var chassisId = new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 };
            var vehicle = new Vehicle(vehicleType.Car, chassisId, "Vermelho");
            _vehicleServiceMock.Setup(s => s.InsertVehicle(vehicle));

            // Act
            var result = _controller.CreateVehicle(vehicle);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetVehicleDetails), createdResult.ActionName);
            Assert.Equal(vehicle, createdResult.Value);

            // Verificar se os route values estão corretos
            Assert.Equal("ABC", createdResult.RouteValues["series"]);
            Assert.Equal(123, createdResult.RouteValues["number"]);

            _vehicleServiceMock.Verify(s => s.InsertVehicle(vehicle), Times.Once);
        }

        [Fact]
        public void CreateVehicle_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var chassisId = new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 };
            var vehicle = new Vehicle(vehicleType.Car, chassisId, "Red");
            _vehicleServiceMock.Setup(s => s.InsertVehicle(vehicle)).Throws(new Exception("Database error"));

            // Act
            var result = _controller.CreateVehicle(vehicle);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Erro interno do server", statusResult.Value.ToString());
        }

        [Fact]
        public void CreateVehicle_CallsInsertVehicle_WithCorrectVehicleParameter()
        {
            // Arrange
            var chassisId = new ChassisIdentifier { ChassisSeries = "TEST", ChassisNumber = 555 };
            var vehicle = new Vehicle(vehicleType.Truck, chassisId, "Yellow");

            // Act
            var result = _controller.CreateVehicle(vehicle);

            // Assert
            _vehicleServiceMock.Verify(s => s.InsertVehicle(It.Is<Vehicle>(v =>
                v.ChassisId.ChassisSeries == "TEST" &&
                v.ChassisId.ChassisNumber == 555 &&
                v.Color == "Yellow" &&
                v.Type == vehicleType.Truck)), Times.Once);
        }

        [Fact]
        public void CreateVehicle_ReturnsCreatedAtAction_WithCorrectRouteValues()
        {
            // Arrange
            var chassisId = new ChassisIdentifier { ChassisSeries = "ROUTE", ChassisNumber = 777 };
            var vehicle = new Vehicle(vehicleType.Bus, chassisId, "Azul");

            // Act
            var result = _controller.CreateVehicle(vehicle);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("ROUTE", createdResult.RouteValues["series"]);
            Assert.Equal(777, createdResult.RouteValues["number"]);
        }

        #endregion

        #region UpdateVehicleColor Tests

        [Fact]
        public void UpdateVehicleColor_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var request = new UpdateVehicleColorRequest
            {
                ChassisSeries = "ABC",
                ChassisNumber = 123,
                NewColor = "Verde"
            };
            var chassisId = new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 };
            var existingVehicle = new Vehicle(vehicleType.Car, chassisId, "Vermelho");
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Returns(existingVehicle);
            _vehicleServiceMock.Setup(s => s.UpdateVehicle(It.IsAny<ChassisIdentifier>(), request.NewColor));

            // Act
            var result = _controller.UpdateVehicleColor(request);

            // Assert
            Assert.IsType<OkResult>(result);
            _vehicleServiceMock.Verify(s => s.UpdateVehicle(It.IsAny<ChassisIdentifier>(), request.NewColor), Times.Once);
        }

        [Fact]
        public void UpdateVehicleColor_ReturnsNotFound_WhenVehicleDoesNotExist()
        {
            // Arrange
            var request = new UpdateVehicleColorRequest
            {
                ChassisSeries = "DEF",
                ChassisNumber = 456,
                NewColor = "Verde"
            };
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Returns((Vehicle)null);

            // Act
            var result = _controller.UpdateVehicleColor(request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _vehicleServiceMock.Verify(s => s.UpdateVehicle(It.IsAny<ChassisIdentifier>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void UpdateVehicleColor_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var request = new UpdateVehicleColorRequest
            {
                ChassisSeries = "ABC",
                ChassisNumber = 123,
                NewColor = "Verde"
            };
            var chassisId = new ChassisIdentifier { ChassisSeries = "ABC", ChassisNumber = 123 };
            var existingVehicle = new Vehicle(vehicleType.Car, chassisId, "Vermelho");
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Returns(existingVehicle);
            _vehicleServiceMock.Setup(s => s.UpdateVehicle(It.IsAny<ChassisIdentifier>(), request.NewColor)).Throws(new Exception("Database error"));

            // Act
            var result = _controller.UpdateVehicleColor(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Erro interno do server", statusResult.Value.ToString());
        }

        [Fact]
        public void UpdateVehicleColor_CreatesCorrectChassisIdentifier_FromRequest()
        {
            // Arrange
            var request = new UpdateVehicleColorRequest
            {
                ChassisSeries = "UPDATE",
                ChassisNumber = 888,
                NewColor = "Vermelho"
            };
            var vehicle = new Vehicle(vehicleType.Car, new ChassisIdentifier { ChassisSeries = "UPDATE", ChassisNumber = 888 }, "Red");

            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.Is<ChassisIdentifier>(
                c => c.ChassisSeries == "UPDATE" && c.ChassisNumber == 888)))
                .Returns(vehicle);

            // Act
            var result = _controller.UpdateVehicleColor(request);

            // Assert
            Assert.IsType<OkResult>(result);
            _vehicleServiceMock.Verify(s => s.GetVehicleByChassi(It.Is<ChassisIdentifier>(
                c => c.ChassisSeries == "UPDATE" && c.ChassisNumber == 888)), Times.Once);
            _vehicleServiceMock.Verify(s => s.UpdateVehicle(It.Is<ChassisIdentifier>(
                c => c.ChassisSeries == "UPDATE" && c.ChassisNumber == 888), "Vermelho"), Times.Once);
        }

        [Fact]
        public void UpdateVehicleColor_CallsUpdateVehicle_WithCorrectParameters()
        {
            // Arrange
            var request = new UpdateVehicleColorRequest
            {
                ChassisSeries = "PARAM",
                ChassisNumber = 999,
                NewColor = "Rosa"
            };
            var vehicle = new Vehicle(vehicleType.Truck, new ChassisIdentifier { ChassisSeries = "PARAM", ChassisNumber = 999 }, "Azul");
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Returns(vehicle);

            // Act
            var result = _controller.UpdateVehicleColor(request);

            // Assert
            _vehicleServiceMock.Verify(s => s.UpdateVehicle(
                It.Is<ChassisIdentifier>(c => c.ChassisSeries == "PARAM" && c.ChassisNumber == 999),
                "Rosa"), Times.Once);
        }

        [Fact]
        public void UpdateVehicleColor_ReturnsInternalServerError_WhenGetVehicleThrowsException()
        {
            // Arrange
            var request = new UpdateVehicleColorRequest
            {
                ChassisSeries = "ERROR",
                ChassisNumber = 111,
                NewColor = "Preto"
            };
            _vehicleServiceMock.Setup(s => s.GetVehicleByChassi(It.IsAny<ChassisIdentifier>())).Throws(new Exception("Database connection failed"));

            // Act
            var result = _controller.UpdateVehicleColor(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Erro interno do server", statusResult.Value.ToString());
            _vehicleServiceMock.Verify(s => s.UpdateVehicle(It.IsAny<ChassisIdentifier>(), It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
