3# notes

## Notes on shortcuts/lessons learned for different topics

# To start the services, run: docker-compose up -d
# To manually create the network beforehand, run: docker network create mynetwork


from django.urls import path, include
from rest_framework.routers import DefaultRouter
from myapp.views import RootCauseViewSet

router = DefaultRouter()
router.register(r'root-causes', RootCauseViewSet, basename='rootcause')

urlpatterns = [
    path('', include(router.urls)),
]

