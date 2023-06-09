﻿using KpopZtationLab.Factory;
using KpopZtationLab.Models;
using KpopZtationLab.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KpopZtationLab.Handler
{
    public class CartHandler
    {

        public static void add(int customerID, int albumID, int quantity)
        {
            var albumToCart = repo.carts.Find(x => x.CustomerID == customerID && x.AlbumID == albumID).FirstOrDefault();
            if (albumToCart != null)
            {
                albumToCart.Qty += quantity;
                repo.carts.Update(albumToCart);
            }
            else
            {
                var newUserAlbum = CartFactory.Create(customerID, albumID, quantity);
                repo.carts.Add(newUserAlbum);
            }
        }

        public static void remove(int userID, int albumID)
        {
            var cartItemTobeDeleted = repo.carts.Find(x => x.CustomerID == userID && x.AlbumID == albumID).FirstOrDefault();
            if (cartItemTobeDeleted != null)
            {
                repo.carts.Remove(cartItemTobeDeleted);
            }
        }

        public static List<Cart> getCarts(int id)
        {
            return repo.carts.Find(x => x.CustomerID == id).ToList();
        }

        public static void checkOut(int id)
        {
            var cartTobeCheckout = repo.carts.Find(x => x.CustomerID == id).ToList();
            var transactionHeader = TransactionHeaderFactory.Create(id, DateTime.Now);
            repo.transactionHeaders.Add(transactionHeader);
            foreach (var cartItem in cartTobeCheckout)
            {
                var transactionDetail = TransactionDetailsFactory.Create(transactionHeader.TransactionID, cartItem.AlbumID, cartItem.Qty);
                //do update to the quantity
                var decreaseAlbumQty = repo.albums.Find(x => x.AlbumID == cartItem.AlbumID).FirstOrDefault();
                decreaseAlbumQty.AlbumStock -= cartItem.Qty;
                repo.albums.Update(decreaseAlbumQty);
                repo.transactionDetails.Add(transactionDetail);
            }
            repo.carts.RemoveRange(cartTobeCheckout);
        }

    }
}