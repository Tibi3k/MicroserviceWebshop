const express = require('express')
const passport = require('passport');
const config = require('./config.json');

const BearerStrategy = require('passport-azure-ad').BearerStrategy;

const options = {
    identityMetadata: `https://${config.credentials.tenantName}.b2clogin.com/${config.credentials.tenantName}.onmicrosoft.com/${config.policies.policyName}/${config.metadata.version}/${config.metadata.discovery}`,
    clientID: config.credentials.clientID,
    audience: config.credentials.clientID,
    policyName: config.policies.policyName,
    isB2C: config.settings.isB2C,
    validateIssuer: config.settings.validateIssuer,
    loggingLevel: config.settings.loggingLevel,
    passReqToCallback: config.settings.passReqToCallback
}

const bearerStrategy = new BearerStrategy(options, (token, done) => {
        // Send user info using the second argument
        done(null, { }, token);
    }
);

const app = express();

app.use(express.json()); 

app.use(passport.initialize());

passport.use(bearerStrategy);

app.get('/auth',
    passport.authenticate('oauth-bearer', {session: false}),
    (req, res) => {
        res.header('Email', base64Encode(req.authInfo.emails[0]))
        res.header('UserId',  base64Encode(req.authInfo.oid))
        res.header('UserName', base64Encode(req.authInfo.family_name + req.authInfo.given_name))
        if(req.authInfo.jobTitle != undefined)
            res.header('jobTitle', base64Encode(req.authInfo.jobTitle))
        else 
            res.header('jobTitle', base64Encode(""))
        res.status(200).send()
    }
);

const base64Encode = (value) => {
    return Buffer.from(value).toString('base64')

}

const port = config.port;

app.listen(port, () => {
    console.log('Listening on port ' + port);
});