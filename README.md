# CCStar
Currency Converter using "Breadth-First Search" algorithm with .net console app.

a configurable module that given a list of currency exchange rates, converts currencies to one
another. Assume we have the following conversion rates:
- (USD => CAD) 2.42
- (CAD => GBP) 0.62
- (USD => EUR) 0.88


If we want to convert USD to EUR it’s pretty straight forward, we just multiply the amount by the
conversion rate which is XX. Also, to convert CAD to USD we may divide the amount by the conversion
rate which is YY. On the other hand, notice that if we want to convert CAD to EUR there’s no direct way
to do so. Our currency converter should be able to do this by finding a “conversion path”,. The
conversion path requires our converter in order to convert CAD to EUR to first convert the amount to USD,
and then to EUR.
▪ The converter find the shortest conversion path (if any).
▪ The converter is a singleton with potentially multiple threads invoking its Convert method.
