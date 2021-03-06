﻿using System;
using GroupMessage.Server.Model;
using NUnit.Framework;
using Nancy;
using System.Collections.Generic;
using System.Linq;

namespace GroupMessage.Server.Test.Module
{
    [TestFixture]
    public class UserModuleTest : NancyIntegrationTestBase<User>
    {
        [Test]
        public void GET_ShouldReturnStatusOKAndAllUsers()
        {
            // ARRANGE
            Db.EntityCollection.Insert(new User {Name = "Name1", LastName = "Lastname1", PhoneNumber = "11111111", Email = "email1@mail.dk"});
            Db.EntityCollection.Insert(new User {Name = "Name2", LastName = "Lastname2", PhoneNumber = "22222222", Email = "email2@mail.dk"});

            // ACT
            var response = Browser.Get("/groupmessage/user");

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var users = response.Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users.Count(user => user.Name == "Name1"), Is.EqualTo(1));
            Assert.That(users.Count(user => user.Name == "Name2"), Is.EqualTo(1));
        }
        
        [Test]
        public void PUT_PhoneUpdate_UserWithExistingPhoneNumber_ShouldReturnStatusOkAndUpdateUserWithDeviceData()
        {
            // ARRANGE
            Db.EntityCollection.Insert(new User { Name = "Name1", LastName = "Lastname1", PhoneNumber = "11111111", Email = "email1@mail.dk" });
            var phoneUserUpdate = new User { PhoneNumber = "11111111", DeviceOs = DeviceOs.Android, DeviceOsVersion = "4.0.1", DeviceToken = "someTokenValue" };

            // ACT
            var response = Browser.Put(string.Format("/groupmessage/user/{0}/", phoneUserUpdate.PhoneNumber), phoneUserUpdate.AsJson());

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(1));
            var userRetuned = users.Single();
            Assert.That(userRetuned.DeviceToken, Is.EqualTo("someTokenValue"));
            Assert.That(userRetuned.DeviceOs, Is.EqualTo(DeviceOs.Android));
            Assert.That(userRetuned.DeviceOsVersion, Is.EqualTo("4.0.1"));
            Assert.That(userRetuned.LastUpdate, Is.InRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow));
        }

        [Test]
        public void PUT_PhoneUpdate_UserWithUnknownPhoneNumber_ShouldReturnStatusBadRequest()
        {
            // ARRANGE
            var phoneUpdateWithUnkownPhoneNumber = new User { PhoneNumber = "11111111", DeviceOs = DeviceOs.Android, DeviceToken = "someTokenValue" };

            // ACT
            var response = Browser.Put(string.Format("/groupmessage/user/{0}/", phoneUpdateWithUnkownPhoneNumber.PhoneNumber), phoneUpdateWithUnkownPhoneNumber.AsJson());

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(0));
        }

        [Test]
        public void PUT_PhoneUpdate_IllegalJson_ShouldReturnStatusBadRequest()
        {
            // ARRANGE
            // ACT
            var response = Browser.Put("/groupmessage/user/11111111", "some invalid json");

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void PUT_UserWithUnknownPhoneNumber_ShouldReturnStatusCreatedAndPersistUser()
        {
            // ARRANGE
            var newUser = new User { Name = "Name1", LastName = "Lastname1", PhoneNumber = "11111111", Email = "email1@mail.dk" };
           
            // ACT
            var response = Browser.Put("/groupmessage/user", newUser.AsJson());

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(users.Single().Name, Is.EqualTo("Name1")); 
            Assert.That(users.Single().LastUpdate, Is.InRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow));
        }
        
        [Test]
        public void PUT_UserWithExistingPhoneNumber_ShouldReturnStatusOkAndUpdateUser()
        {
            // ARRANGE
            Db.EntityCollection.Insert(new User { Name = "Name1", LastName = "Lastname1", PhoneNumber = "11111111", Email = "email1@mail.dk" });
            var updatedUser = new User { Name = "Name2", LastName = "Lastname2", PhoneNumber = "11111111", Email = "email2@mail.dk" };

            // ACT
            var response = Browser.Put("/groupmessage/user", updatedUser.AsJson());

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(users.Single().Name, Is.EqualTo("Name2"));
            Assert.That(users.Single().LastUpdate, Is.InRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow));
        }

        [Test]
        public void PUT_UserWithNoPhoneNumber_ShouldReturnStatusBadRequest()
        {
            // ARRANGE
            var userWithNoPhoneNumber = new User { Name = "Name", LastName = "Lastname", Email = "email@mail.dk" };

            // ACT
            var response = Browser.Put("/groupmessage/user", userWithNoPhoneNumber.AsJson());

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(0));
        }

        [Test]
        public void PUT_InvalidJsonData_ShouldReturnStatusBadRequest()
        {
            // ARRANGE
            // ACT
            var response = Browser.Put("/groupmessage/user", "some invalid json");

            // ASSERT
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var users = Browser.Get("/groupmessage/user").Body.UnmarshallJson<List<User>>();
            Assert.That(users.Count, Is.EqualTo(0));
        }
    }
}